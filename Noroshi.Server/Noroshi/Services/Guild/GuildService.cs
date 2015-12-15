using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response.Guild;
using Noroshi.Core.Game.GameContent;
using Noroshi.Core.Game.Guild;
using Constant = Noroshi.Core.Game.Guild.Constant;
using Noroshi.Core.Game.Player;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.FrameWork;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services.Guild
{
    public class GuildService : AbstractGuildService
    {
        /// <summary>
        /// 一度に取得するおすすめギルド数。
        /// </summary>
        const byte RECOMMENDED_NUM = 3;

        public static GetOwnResponse GetOwn(uint playerId)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            if (!GameContent.IsOpen(GameContentID.BeginnerGuild, playerStatus.Level))
            {
                throw new InvalidOperationException(string.Join("\t", "Not Open", playerStatus.PlayerID));
            }
            if (!playerStatus.GuildID.HasValue)
            {
                return new GetOwnResponse { Error = new GuildError { GuildNotFound = true } };
            }
            // ギルド取得。
            var guild = GuildEntity.ReadAndBuild(playerStatus.GuildID.Value);
            if (guild == null)
            {
                throw new SystemException(string.Join("\t", "Guild Not Found", playerStatus.GuildID.Value));
            }
            var guildMembers = guild.GetMemberPlayerStatuses();

            // ギルドリクエスト取得。
            var guildRequests = GuildRequestEntity.ReadAndBuildByGuildID(guild.ID);
            var ownGuildRequest = GuildRequestEntity.ReadAndBuild(playerId);
            var playerStatusMap = guildRequests.Count() > 0 ? PlayerStatusEntity.ReadAndBuildMulti(guildRequests.Select(gr => gr.PlayerID)).ToDictionary(ps => ps.PlayerID) : new Dictionary<uint, PlayerStatusEntity>();
            var requestingGuild = ownGuildRequest != null ? GuildEntity.ReadAndBuild(ownGuildRequest.GuildID) : null;

            // 挨拶関連情報取得。
            var targetIdToPlayerRelationMap = PlayerRelationEntity.ReadOrDefaultAndBuildMulti(playerId, guildMembers.Select(ps => ps.PlayerID)).ToDictionary(pr => pr.TargetPlayerID);
            var playerIdToLastGreetedConfirmedAtMap = PlayerConfirmationEntity.ReadLastGreetedConfirmedAtMulti(guildMembers.Select(ps => ps.PlayerID));

            return new GetOwnResponse
            {
                Guild = guild.ToResponseData(),
                GuildMembers = guildMembers.Select(ps => ps.ToGuildMemberResponseData(targetIdToPlayerRelationMap[ps.PlayerID].CanGreet(playerStatus, ps, guild, playerIdToLastGreetedConfirmedAtMap[ps.PlayerID]))).ToArray(),
                Requests = guildRequests.Select(gr => gr.ToResponseData(playerStatusMap[gr.PlayerID])).ToArray(),
                RequestingGuild = requestingGuild?.ToResponseData(),
                MaxGreetingNum = guild.GetMaxGreetingNum(playerStatus.VipLevel),
                GreetingNum = playerStatus.GreetingNum,
                UnconfirmedGreetedNum = playerStatus.UnconfirmedGreetedNum,
            };
        }
        /// <summary>
        /// ギルドを ID 指定で取得する。
        /// </summary>
        /// <param name="playerId">取得者プレイヤー ID</param>
        /// <param name="guildId">対象ギルド ID</param>
        /// <returns></returns>
        public static GetResponse Get(uint playerId, uint guildId)
        {
            var guild = GuildEntity.ReadAndBuild(guildId);
            if (guild == null)
            {
                return new GetResponse { Error = new GuildError { GuildNotFound = true } };
            }
            var leader = PlayerStatusEntity.ReadAndBuild(guild.LeaderPlayerID);
            return new GetResponse
            {
                Guild = guild.ToResponseData(),
                Leader = leader.ToOtherResponseData(),
            };
        }
        /// <summary>
        /// おすすめギルドを取得する。
        /// </summary>
        /// <param name="playerId">検索者プレイヤー ID</param>
        /// <returns></returns>
        public static GetRecommendedGuildsResponse GetRecommendedGuilds(uint playerId)
        {
            // 対象プレイヤーをビルド。
            var playerStatus = _buildOwnPlayerStatus(playerId, ReadType.Slave);
            // おすすめギルドのビルドを試みる。
            var recommendedGuilds = GuildEntity.ReadAndBuildRecommendedGuilds(playerStatus, RECOMMENDED_NUM);

            return new GetRecommendedGuildsResponse
            {
                Guilds = recommendedGuilds.Select(guild => guild.ToResponseData()).ToArray(),
            };
        }

        /// <summary>
        /// 該当プレイヤーを初心者ギルドへ加入させる。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <returns></returns>
        public static JoinResponse JoinBeginnerGuild(uint playerId)
        {
            // SLAVE チェック。
            var response = _validateWhenJoinBeginnerGuild(playerId, ReadType.Slave, (playerStatus, beginnerGuild, error) =>
            {
                return new JoinResponse { Error = error };
            });
            if (response.Error != null) return response;

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenJoinBeginnerGuild(playerId, ReadType.Lock, (playerStatus, beginnerGuild, error) =>
                {
                    if (error != null) return new JoinResponse { Error = error };

                    // 加入処理。
                    beginnerGuild.Join(playerStatus, null, null, null);
                    // 更新実行。
                    if (!playerStatus.Save() || !beginnerGuild.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save", playerStatus.PlayerID, beginnerGuild.ID));
                    }
                    tx.Commit();
                    return new JoinResponse { NewGuild = beginnerGuild.ToResponseData() };
                });
            });
        }
        static T _validateWhenJoinBeginnerGuild<T>(uint playerId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, GuildError, T> func)
        {
            return _build(playerId, readType, (playerStatus, currentGuild, guildRequest, requestGuild) =>
            {
                if (requestGuild != null) throw new InvalidOperationException();

                // 初心者ギルドをビルド。ビルドが成功すれば加入チェック。
                var beginnerGuild = GuildEntity.ReadAndBuildBeginnerGuild(readType);
                if (beginnerGuild != null)
                {
                    if (!beginnerGuild.CanJoinAsMember(playerStatus, currentGuild))
                    {
                        throw new InvalidOperationException(string.Join("\t", "Cannot Join Beginner Guild", playerStatus.PlayerID, beginnerGuild.ID));
                    }
                }
                else
                {
                    // ロック指定時はレコード作成も含むので確実にビルドできるはず。
                    if (readType == ReadType.Lock)
                    {
                        throw new SystemException(string.Join("\t", "Beginner Guild Not Found", playerStatus.PlayerID));
                    }
                }
                return func.Invoke(playerStatus, beginnerGuild, null);
            });
        }

        /// <summary>
        /// 該当プレイヤーを自動的にギルド移籍させる。
        /// </summary>
        /// <param name="playerId">対象プレイヤー</param>
        /// <returns></returns>
        public static JoinAutomaticallyResponse JoinAutomatically(uint playerId, uint countryId)
        {
            // 対象プレイヤーをビルド。
            var playerStatus = _buildOwnPlayerStatus(playerId, ReadType.Slave);
            // まずはおすすめギルドのビルドを試みる。
            var recommendedGuild = GuildEntity.ReadAndBuildRecommendedGuilds(playerStatus, 1).FirstOrDefault();

            // おすすめギルドがあれば異動。
            if (recommendedGuild != null)
            {
                var response = Join(playerStatus.PlayerID, recommendedGuild.ID);
                return new JoinAutomaticallyResponse
                {
                    NewGuild = response.NewGuild,
                    Error = response.Error,
                };
            }
            // おすすめギルドがなければ新規ギルド作成。
            else
            {
                // デフォルト文言は DynamicText から取得。
                var textMap = DynamicTextEntity.ReadAndBuildByLanguageIDAndTextKeys(playerStatus.LanguageID, new[]
                {
                    Constant.DEFAULT_GUILD_NAME_FORMAT_TEXT_KEY,
                    Constant.DEFAULT_GUILD_INTRODUCTION_FORMAT_TEXT_KEY,
                })
                .ToDictionary(text => text.TextKey);
                var defaultName = string.Format(textMap[Constant.DEFAULT_GUILD_NAME_FORMAT_TEXT_KEY].Text, playerStatus.Name);
                var defaultDescription = string.Format(textMap[Constant.DEFAULT_GUILD_INTRODUCTION_FORMAT_TEXT_KEY].Text, playerStatus.Name);
                var defaultNecessaryPlayerLevel = GameContent.GetNormalGuildOpenPlayerLevel();

                var response = Create(playerStatus.PlayerID, true, countryId, defaultNecessaryPlayerLevel, defaultName, defaultDescription, true);
                return new JoinAutomaticallyResponse
                {
                    NewGuild = response.NewGuild,
                    Error = response.Error,
                };
            }
        }

        /// <summary>
        /// ギルド設立。
        /// </summary>
        /// <param name="playerId">設立者プレイヤー ID</param>
        /// <param name="isOpen">オープン or クローズ</param>
        /// <param name="countryId">国籍 ID</param>
        /// <param name="necessaryPlayerLevel">必要プレイヤーレベル</param>
        /// <param name="name">ギルド名</param>
        /// <param name="introduction">紹介文</param>
        /// <param name="withoutPayment">支払い有無</param>
        /// <returns></returns>
        public static CreateResponse Create(uint playerId, bool isOpen, uint countryId, ushort necessaryPlayerLevel, string name, string introduction, bool withoutPayment = false)
        {
            // 国籍チェック。
            if (!Enum.IsDefined(typeof(Country), (Country)countryId))
            {
                throw new InvalidOperationException(string.Join("\t", "Invalid Country", playerId, countryId));
            }
            // 最低プレイヤーレベルチェック。
            if (!GuildEntity.IsValidNecessaryPlayerLevel(necessaryPlayerLevel))
            {
                throw new InvalidOperationException(string.Join("\t", "Invalid Necessary Player Level", playerId, necessaryPlayerLevel));
            }
            // ギルド名チェック。
            if (!GuildEntity.IsValidName(name))
            {
                throw new InvalidOperationException(string.Join("\t", "Invalid Name", playerId, name));
            }
            // 紹介文チェック。
            if (!GuildEntity.IsValidIntroduction(introduction))
            {
                throw new InvalidOperationException(string.Join("\t", "Invalid Introduction", playerId, introduction));
            }
            var payment = withoutPayment ? new PossessionParam[0] : new[] { GuildEntity.GetPaymentToCreateNormalGuild() };

            // SLAVE チェック。
            var response = _validateWhenCreate(playerId, payment, ReadType.Slave, (playerStatus, currentGuild, request, requestingGuild, possessionManager, error) =>
            {
                return new CreateResponse { Error = error };
            });
            if (response.Error != null) return response;

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenCreate(playerId, payment, ReadType.Lock, (playerStatus, currentGuild, request, requestingGuild, possessionManager, error) =>
                {
                    if (error != null) return new CreateResponse { Error = error };

                    // 新規ギルド設立。
                    var nextGuild = GuildEntity.CreateNormalGuild(playerStatus, currentGuild, request, requestingGuild, isOpen, countryId, necessaryPlayerLevel, name, introduction);
                    // 保存実行。
                    if (!playerStatus.Save() || !nextGuild.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save", playerStatus.PlayerID, nextGuild.ID));
                    }
                    if (currentGuild != null && currentGuild.HasRecord && !currentGuild.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save Guild", currentGuild.ID));
                    }
                    // 設立支払い。
                    if (!withoutPayment)
                    {
                        possessionManager.Remove(GuildEntity.GetPaymentToCreateNormalGuild());
                    }

                    tx.Commit();
                    return new CreateResponse { NewGuild = nextGuild.ToResponseData() };
                });
            });
        }
        static T _validateWhenCreate<T>(uint playerId, IEnumerable<PossessionParam> payment, ReadType readType, Func<PlayerStatusEntity, GuildEntity, GuildRequestEntity, GuildEntity, PossessionManager, GuildError, T> func)
        {
            return _build(playerId, readType, (playerStatus, currentGuild, guildRequest, requestGuild) =>
            {
                if (!GuildEntity.CanCreateNormalGuild(playerStatus, currentGuild))
                {
                    throw new InvalidOperationException(string.Join("\t", "Cannot Create Normal Guild", playerStatus.PlayerID, currentGuild.ID));
                }
                var possessionManager = new PossessionManager(playerId, payment);
                if (readType == ReadType.Slave)
                {
                    possessionManager.Load();
                    if (!possessionManager.CanRemove(payment))
                    {
                        throw new InvalidOperationException(string.Join("\t", "Cannot Pay to Create Normal Guild", playerStatus.PlayerID));
                    }
                }
                return func.Invoke(playerStatus, currentGuild, guildRequest, requestGuild, possessionManager, null);
            });
        }

        /// <summary>
        /// ギルドに加入する。
        /// </summary>
        /// <param name="playerId">加入プレイヤーID</param>
        /// <param name="nextGuildId">加入ギルド ID</param>
        /// <returns></returns>
        public static JoinResponse Join(uint playerId, uint nextGuildId)
        {
            // SLAVE チェック。
            var response = _validateWhenJoin(playerId, nextGuildId, ReadType.Slave, (playerStatus, currentGuild, guildRequest, requestGuild, nextGuild, error) =>
            {
                return new JoinResponse { Error = error };
            });
            if (response.Error != null) return response;

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenJoin(playerId, nextGuildId, ReadType.Lock, (playerStatus, currentGuild, guildRequest, requestGuild, nextGuild, error) =>
                {
                    if (error != null) return new JoinResponse { Error = error };

                    // 新ギルド参加。
                    nextGuild.Join(playerStatus, currentGuild, guildRequest, requestGuild);
                    // 保存実行。
                    if (!playerStatus.Save() || !nextGuild.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save", playerStatus.PlayerID, nextGuild.ID));
                    }
                    if (currentGuild != null && currentGuild.HasRecord && !currentGuild.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save Guild", currentGuild.ID));
                    }

                    tx.Commit();
                    return new JoinResponse { NewGuild = nextGuild.ToResponseData() };
                });
            });
        }
        static T _validateWhenJoin<T>(uint playerId, uint targetGuildId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, GuildRequestEntity, GuildEntity, GuildEntity, GuildError, T> func)
        {
            return _validateWithOtherGuild(playerId, targetGuildId, readType, (playerStatus, currentGuild, guildRequest, requestGuild, targetGuild, error) =>
            {
                if (!targetGuild.CanJoinAsMember(playerStatus, currentGuild))
                {
                    return func.Invoke(playerStatus, currentGuild, guildRequest, requestGuild, targetGuild, new GuildError { CannotJoin = true });
                }
                return func.Invoke(playerStatus, currentGuild, guildRequest, requestGuild, targetGuild, null);
            });
        }

        /// <summary>
        /// 加入リクエスト。
        /// </summary>
        /// <param name="playerId">リクエスト実行者プレイヤー ID</param>
        /// <param name="targetGuildId">対象ギルド ID</param>
        /// <returns></returns>
        public static HandleRequestResponse Request(uint playerId, uint targetGuildId)
        {
            var response = _validateWhenRequest(playerId, targetGuildId, ReadType.Slave, (playerStatus, targetGuild, error) =>
            {
                return new HandleRequestResponse { Error = error };
            });
            if (response.Error != null) return response;

            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenRequest(playerId, targetGuildId, ReadType.Lock, (playerStatus, targetGuild, error) =>
                {
                    if (error != null) return new HandleRequestResponse { Error = error };

                    if (GuildRequestEntity.Create(playerStatus.PlayerID, targetGuild.ID) == null)
                    {
                        throw new SystemException(string.Join("\t", "Cannot Create Guild Request", playerStatus.PlayerID, targetGuild.ID));
                    }
                    targetGuild.ReceiveRequest(playerStatus);
                    if (!playerStatus.Save() || !targetGuild.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save", playerStatus.PlayerID, targetGuild.ID));
                    }
                    tx.Commit();

                    return new HandleRequestResponse { Guild = targetGuild.ToResponseData() };
                });
            });
        }
        static T _validateWhenRequest<T>(uint playerId, uint targetGuildId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, GuildError, T> func)
        {
            return _validateWithOtherGuild(playerId, targetGuildId, readType, (playerStatus, currentGuild, guildRequest, requestGuild, targetGuild, error) =>
            {
                if (currentGuild == null)
                {
                    return func.Invoke(playerStatus, targetGuild, new GuildError { GuildNotFound = true });
                }
                // リクエスト可否チェック
                if (!targetGuild.CanReceiveRequest(playerStatus, currentGuild))
                {
                    return func.Invoke(playerStatus, targetGuild, new GuildError { CannotRequest = true });
                }
                return func.Invoke(playerStatus, targetGuild, null);
            });
        }

        /// <summary>
        /// 加入リクエストをキャンセルする。
        /// </summary>
        /// <param name="playerId">キャンセル実行者プレイヤーID</param>
        /// <returns></returns>
        public static HandleRequestResponse CancelRequest(uint playerId)
        {
            var response = _validateWhenCancelRequest(playerId, ReadType.Slave, (playerStatus, request, requestGuild, error) =>
            {
                return new HandleRequestResponse { Error = error, Guild = requestGuild?.ToResponseData() };
            });
            if (response.Error != null) return response;

            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenCancelRequest(playerId, ReadType.Lock, (playerStatus, request, requestGuild, error) =>
                {
                    if (error != null) return new HandleRequestResponse { Error = error, Guild = requestGuild?.ToResponseData() };

                    if (requestGuild != null)
                    {
                        requestGuild.DeleteRequest(playerStatus, request);
                        if (!playerStatus.Save() || !requestGuild.Save())
                        {
                            throw new SystemException(string.Join("\t", "Fail to Save", playerStatus.PlayerID, requestGuild.ID));
                        }
                    }
                    else
                    {
                        playerStatus.SetGuildState(PlayerGuildState.Default);
                        if (!playerStatus.Save() || !request.Delete())
                        {
                            throw new SystemException(string.Join("\t", "Fail to Cancel", playerStatus.PlayerID, requestGuild.ID));
                        }
                    }
                    tx.Commit();

                    return new HandleRequestResponse { Guild = requestGuild.ToResponseData() };
                });
            });
        }
        static T _validateWhenCancelRequest<T>(uint playerId, ReadType readType, Func<PlayerStatusEntity, GuildRequestEntity, GuildEntity, GuildError, T> func)
        {
            var playerStatus = _buildOwnPlayerStatus(playerId, readType);
            var request = playerStatus.GuildState == PlayerGuildState.Request ? GuildRequestEntity.ReadAndBuild(playerId, readType) : null;
            if (request == null)
            {
                return func.Invoke(playerStatus, request, null, new GuildError { NoRequest = true });
            }
            // 解散している可能性あり。
            var requestGuild = GuildEntity.ReadAndBuild(request.GuildID, readType);
            return func.Invoke(playerStatus, request, requestGuild, null);
        }

        /// <summary>
        /// 加入リクエストを受諾する。
        /// </summary>
        /// <param name="playerId">受諾実行者プレイヤー ID</param>
        /// <param name="targetPlayerId">受諾対象プレイヤー ID</param>
        /// <returns></returns>
        public static HandleReceivedRequestResponse AcceptRequest(uint playerId, uint targetPlayerId)
        {
            var response = _validateWhenAcceptRequest(playerId, targetPlayerId, ReadType.Slave, (playerStatus, guild, targetPlayerStatus, targetGuild, targetRequest, targetRequestingGuild, error) =>
            {
                return new HandleReceivedRequestResponse { Requester = targetPlayerStatus?.ToOtherResponseData(), Error = error };
            });
            if (response.Error != null) return response;

            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenAcceptRequest(playerId, targetPlayerId, ReadType.Lock, (playerStatus, guild, targetPlayerStatus, targetGuild, targetRequest, targetRequestingGuild, error) =>
                {
                    if (error != null) response = new HandleReceivedRequestResponse { Requester = targetPlayerStatus?.ToOtherResponseData(), Error = error };

                    guild.AcceptRequest(targetPlayerStatus, targetGuild, targetRequest, targetRequestingGuild);
                    if (!targetPlayerStatus.Save() || !guild.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save", targetPlayerStatus.PlayerID, guild.ID));
                    }
                    tx.Commit();

                    return new HandleReceivedRequestResponse { Requester = targetPlayerStatus.ToOtherResponseData() };
                });
            });
        }
        static T _validateWhenAcceptRequest<T>(uint playerId, uint targetPlayerId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, PlayerStatusEntity, GuildEntity, GuildRequestEntity, GuildEntity, GuildError, T> func)
        {
            return _validateWhenHandleReceivedRequest(playerId, targetPlayerId, readType, (playerStatus, guild, targetPlayerStatus, targetGuild, targetRequest, targetRequestingGuild, error) =>
            {
                if (error != null) func.Invoke(playerStatus, guild, targetPlayerStatus, targetGuild, targetRequest,targetRequestingGuild, error);

                // 加入リクエスト受諾可否チェック
                if (!guild.CanAcceptRequest(targetPlayerStatus, targetGuild, targetRequest))
                {
                    return func.Invoke(playerStatus, guild, targetPlayerStatus, targetGuild, targetRequest, targetRequestingGuild, new GuildError { CannotAcceptRequest = true });
                }
                return func.Invoke(playerStatus, guild, targetPlayerStatus, targetGuild, targetRequest, targetRequestingGuild, null);
            });
        }

        /// <summary>
        /// 加入リクエストを拒否する。
        /// </summary>
        /// <param name="playerId">拒否実行者プレイヤー ID</param>
        /// <param name="targetPlayerId">拒否対象プレイヤー ID</param>
        /// <returns></returns>
        public static HandleReceivedRequestResponse RejectRequest(uint playerId, uint targetPlayerId)
        {
            var response = _validateWhenRejectRequest(playerId, targetPlayerId, ReadType.Slave, (playerStatus, guild, targetPlayerStatus, targetGuild, targetRequest, targetRequestGuild, error) =>
            {
                return new HandleReceivedRequestResponse { Requester = targetPlayerStatus?.ToOtherResponseData(), Error = error };
            });
            if (response.Error != null) return response;

            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenRejectRequest(playerId, targetPlayerId, ReadType.Lock, (playerStatus, guild, targetPlayerStatus, targetGuild, targetRequest, targetRequestGuild, error) =>
                {
                    if (error != null) response = new HandleReceivedRequestResponse { Requester = targetPlayerStatus?.ToOtherResponseData(), Error = error };

                    guild.DeleteRequest(targetPlayerStatus, targetRequest);
                    if (!targetPlayerStatus.Save() || !guild.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Reject Request", targetPlayerStatus.PlayerID, guild.ID));
                    }
                    tx.Commit();

                    return new HandleReceivedRequestResponse { Requester = targetPlayerStatus.ToOtherResponseData() };
                });
            });
        }
        static T _validateWhenRejectRequest<T>(uint playerId, uint targetPlayerId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, PlayerStatusEntity, GuildEntity, GuildRequestEntity, GuildEntity, GuildError, T> func)
        {
            return _validateWhenHandleReceivedRequest(playerId, targetPlayerId, readType, (playerStatus, guild, targetPlayerStatus, targetGuild, targetRequest, targetRequestGuild, error) =>
            {
                return func.Invoke(playerStatus, guild, targetPlayerStatus, targetGuild, targetRequest, targetRequestGuild, error);
            });
        }

        static T _validateWhenHandleReceivedRequest<T>(uint playerId, uint targetPlayerId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, PlayerStatusEntity, GuildEntity, GuildRequestEntity, GuildEntity, GuildError, T> func)
        {
            return _buildWithTargetPlayer(playerId, targetPlayerId, readType, (playerStatus, guild, request, requestingGuild, targetPlayerStatus, targetGuild, targetRequest, targetRequestGuild) =>
            {
                // ギルド所属チェック。
                if (guild == null)
                {
                    return func.Invoke(playerStatus, guild, targetPlayerStatus, targetGuild, targetRequest, targetRequestGuild, new GuildError { GuildNotFound = true });
                }
                // 権限チェック。
                if (!playerStatus.GuildRole.HasValue || playerStatus.GuildRole.Value != GuildRole.Executive && playerStatus.GuildRole.Value != GuildRole.Leader)
                {
                    return func.Invoke(playerStatus, guild, targetPlayerStatus, targetGuild, targetRequest, targetRequestGuild, new GuildError { NoAuthority = true });
                }
                // リクエストチェック。
                if (targetRequest == null)
                {
                    return func.Invoke(playerStatus, guild, targetPlayerStatus, targetGuild, targetRequest, targetRequestGuild, new GuildError { NoRequest = true });
                }
                return func.Invoke(playerStatus, guild, targetPlayerStatus, targetGuild, targetRequest, targetRequestGuild, null);
            });
        }
    }
}
