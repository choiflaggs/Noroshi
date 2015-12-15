using System;
using System.Linq;
using Noroshi.Core.Game.Guild;
using Noroshi.Core.Game.Player;
using Noroshi.Core.WebApi.Response.Guild;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services.Guild
{
    /// <summary>
    /// ギルドリーダー用サービスクラス。
    /// </summary>
    public class GuildLeaderService : AbstractGuildService
    {
        /// <summary>
        /// 該当プレイヤーがリーダーを務めるギルドの情報を更新する。
        /// </summary>
        /// <param name="playerId">リーダーのプレイヤーID</param>
        /// <param name="isOpen">変更後のギルドオープン設定（なければ更新しない）</param>
        /// <param name="countryId">変更後の国籍ID（なければ更新しない）</param>
        /// <param name="necessaryPlayerLevel">変更後の最低プレイヤーレベル（なければ更新しない）</param>
        /// <param name="name">変更後のギルド名（なければ更新しない）</param>
        /// <param name="introduction">変更後の紹介文（なければ更新しない）</param>
        /// <returns></returns>
        public static ConfigureResponse Configure(uint playerId, bool? isOpen, uint? countryId, ushort? necessaryPlayerLevel, string name, string introduction)
        {
            // 更新対象有無チェック。
            if (!isOpen.HasValue && !countryId.HasValue && !necessaryPlayerLevel.HasValue && string.IsNullOrEmpty(name) && string.IsNullOrEmpty(introduction))
            {
                throw new InvalidOperationException(string.Join("\t", "No Update Column", playerId));
            }
            // 国籍チェック。
            if (countryId.HasValue && !Enum.IsDefined(typeof(Country), (Country)countryId.Value))
            {
                throw new InvalidOperationException(string.Join("\t", "Invalid Country", playerId, countryId));
            }
            // 最低プレイヤーレベルチェック。
            if (necessaryPlayerLevel.HasValue && !GuildEntity.IsValidNecessaryPlayerLevel(necessaryPlayerLevel.Value))
            {
                throw new InvalidOperationException(string.Join("\t", "Invalid Necessary Player Level", playerId, necessaryPlayerLevel));
            }
            // ギルド名チェック。
            if (!string.IsNullOrEmpty(name) && !GuildEntity.IsValidName(name))
            {
                throw new InvalidOperationException(string.Join("\t", "Invalid Name", playerId, name));
            }
            // 紹介文チェック。
            if (!string.IsNullOrEmpty(introduction) && !GuildEntity.IsValidIntroduction(introduction))
            {
                throw new InvalidOperationException(string.Join("\t", "Invalid Introduction", playerId, introduction));
            }

            // SLAVE チェック。
            var response = _validateWhenConfigure(playerId, isOpen, ReadType.Slave, (playerStatus, guild, error) =>
            {
                return new ConfigureResponse { Error = error };
            });
            if (response.Error != null) return response;

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenConfigure(playerId, isOpen, ReadType.Lock, (playerStatus, guild, error) =>
                {
                    if (error != null) return new ConfigureResponse { Error = error };

                    // 更新対象となっているものをセット
                    if (isOpen.HasValue) guild.SetOpen(isOpen.Value);
                    if (countryId.HasValue) guild.SetCountryID(countryId.Value);
                    if (necessaryPlayerLevel.HasValue) guild.SetNecessaryPlayerLevel(necessaryPlayerLevel.Value);
                    if (!string.IsNullOrEmpty(name)) guild.SetName(name);
                    if (!string.IsNullOrEmpty(introduction)) guild.SetIntroduction(introduction);
                    // 更新実行。
                    if (!guild.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save Guild", playerStatus.PlayerID, guild.ID));
                    }
                    tx.Commit();
                    return new ConfigureResponse { Guild = guild.ToResponseData() };
                });
            });
        }
        static T _validateWhenConfigure<T>(uint playerId, bool? isOpen, ReadType readType, Func<PlayerStatusEntity, GuildEntity, GuildError, T> func)
        {
            return _validateLeader(playerId, readType, (playerStatus, guild, error) =>
            {
                // オープンに変更する場合、申請中のリクエストがあってはいけない。
                if (isOpen.HasValue && isOpen.Value)
                {
                    if (GuildRequestEntity.ReadAndBuildByGuildID(guild.ID, readType).Count() > 0)
                    {
                        return func.Invoke(playerStatus, guild, new GuildError { HasRequest = true });
                    }
                }
                return func.Invoke(playerStatus, guild, error);
            });
        }

        /// <summary>
        /// 該当プレイヤーがリーダーを務めるギルドにて、幹部を任命する。
        /// </summary>
        /// <param name="playerId">リーダーのプレイヤーID</param>
        /// <param name="targetPlayerId">幹部に任命するプレイヤーID</param>
        /// <returns></returns>
        public static AddExecutiveRoleResponse AddExecutiveRole(uint playerId, uint targetPlayerId)
        {
            // SLAVE チェック。
            var response = _validateWhenAddExecutiveRole(playerId, targetPlayerId, ReadType.Slave, (playerStatus, targetPlayerStatus, guild, error) =>
            {
                return new AddExecutiveRoleResponse { Error = error };
            });
            if (response.Error != null) return response;

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenAddExecutiveRole(playerId, targetPlayerId, ReadType.Lock, (playerStatus, targetPlayerStatus, guild, error) =>
                {
                    if (error != null) return new AddExecutiveRoleResponse { Error = error };

                    // 役割セット。
                    targetPlayerStatus.SetGuildRole(GuildRole.Executive);
                    // 更新実行。
                    if (!targetPlayerStatus.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save Player Status", playerStatus.PlayerID, targetPlayerStatus.PlayerID, guild.ID));
                    }
                    tx.Commit();
                    return new AddExecutiveRoleResponse { TargetPlayer = targetPlayerStatus.ToOtherResponseData() };
                });
            });
        }
        static T _validateWhenAddExecutiveRole<T>(uint playerId, uint targetPlayerId, ReadType readType, Func<PlayerStatusEntity, PlayerStatusEntity, GuildEntity, GuildError, T> func)
        {
            return _validateLeaderWithTargetPlayer(playerId, targetPlayerId, readType, (playerStatus, targetPlayerStatus, guild, error) =>
            {
                // 既に役割を持っていてはいけない。
                if (targetPlayerStatus.GuildRole.HasValue)
                {
                    throw new InvalidOperationException(string.Join("\t", "Role Has Already Set", targetPlayerStatus.PlayerID, targetPlayerStatus.GuildRole.Value));
                }
                return func.Invoke(playerStatus, targetPlayerStatus, guild, error);
            });
        }

        /// <summary>
        /// 該当プレイヤーがリーダーを務めるギルドにて、幹部を解任する。
        /// </summary>
        /// <param name="playerId">リーダーのプレイヤーID</param>
        /// <param name="targetPlayerId">幹部から解任されるプレイヤーID</param>
        /// <returns></returns>
        public static RemoveExecutiveRoleResponse RemoveExecutiveRole(uint playerId, uint targetPlayerId)
        {
            // SLAVE チェック。
            var response = _validateWhenRemoveExecutiveRole(playerId, targetPlayerId, ReadType.Slave, (playerStatus, targetPlayerStatus, guild, error) =>
            {
                return new RemoveExecutiveRoleResponse { Error = error };
            });
            if (response.Error != null) return response;

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenRemoveExecutiveRole(playerId, targetPlayerId, ReadType.Lock, (playerStatus, targetPlayerStatus, guild, error) =>
                {
                    if (error != null) return new RemoveExecutiveRoleResponse { Error = error };

                    // 役割削除。
                    targetPlayerStatus.RemoveGuildRole();
                    // 更新実行。
                    if (!targetPlayerStatus.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save Player Status", playerStatus.PlayerID, targetPlayerStatus.PlayerID, guild.ID));
                    }
                    tx.Commit();
                    return new RemoveExecutiveRoleResponse { TargetPlayer = targetPlayerStatus.ToOtherResponseData() };
                });
            });
        }
        static T _validateWhenRemoveExecutiveRole<T>(uint playerId, uint targetPlayerId, ReadType readType, Func<PlayerStatusEntity, PlayerStatusEntity, GuildEntity, GuildError, T> func)
        {
            return _validateLeaderWithTargetPlayer(playerId, targetPlayerId, readType, (playerStatus, targetPlayerStatus, guild, error) =>
            {
                // 該当役割を持っていないと外せない。
                if (!targetPlayerStatus.GuildRole.HasValue || targetPlayerStatus.GuildRole.Value != GuildRole.Executive)
                {
                    throw new InvalidOperationException(string.Join("\t", "Is Not Executive", targetPlayerStatus.PlayerID));
                }
                return func.Invoke(playerStatus, targetPlayerStatus, guild, error);
            });
        }

        /// <summary>
        /// 該当プレイヤーがリーダーを務めるギルドにて、リーダーを他プレイヤーに譲る。
        /// </summary>
        /// <param name="playerId">リーダーのプレイヤーID</param>
        /// <param name="targetPlayerId">新しくリーダーとなるプレイヤーID</param>
        /// <returns></returns>
        public static ChangeLeaderResponse ChangeLeader(uint playerId, uint targetPlayerId)
        {
            // SLAVE チェック。
            var response = _validateWhenChangeLeader(playerId, targetPlayerId, ReadType.Slave, (playerStatus, targetPlayerStatus, guild, error) =>
            {
                return new ChangeLeaderResponse { Error = error };
            });
            if (response.Error != null) return response;

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenChangeLeader(playerId, targetPlayerId, ReadType.Lock, (playerStatus, targetPlayerStatus, guild, error) =>
                {
                    if (error != null) return new ChangeLeaderResponse { Error = error };

                    // リーダー変更。
                    guild.ChangeLeader(playerStatus, targetPlayerStatus);
                    // 更新実行。
                    if (!playerStatus.Save() || !targetPlayerStatus.Save() || !guild.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save", playerStatus.PlayerID, targetPlayerStatus.PlayerID, guild.ID));
                    }
                    tx.Commit();
                    return new ChangeLeaderResponse { NewLeader = targetPlayerStatus.ToOtherResponseData() };
                });
            });
        }
        static T _validateWhenChangeLeader<T>(uint playerId, uint targetPlayerId, ReadType readType, Func<PlayerStatusEntity, PlayerStatusEntity, GuildEntity, GuildError, T> func)
        {
            return _validateLeaderWithTargetPlayer(playerId, targetPlayerId, readType, (playerStatus, targetPlayerStatus, guild, error) =>
            {
                if (!guild.CanChangeLeader(playerStatus, targetPlayerStatus))
                {
                    throw new InvalidOperationException(string.Join("\t", "Cannot Change Leader", playerStatus.PlayerID, targetPlayerStatus.PlayerID, guild.ID));
                }
                return func.Invoke(playerStatus, targetPlayerStatus, guild, error);
            });
        }

        /// <summary>
        /// 該当プレイヤーがリーダーを務めるギルドにて、所属プレイヤーを除名する。
        /// </summary>
        /// <param name="playerId">リーダーのプレイヤーID</param>
        /// <param name="targetPlayerId">除名されるプレイヤーID</param>
        /// <returns></returns>
        public static LayOffResponse LayOff(uint playerId, uint targetPlayerId)
        {
            // SLAVE チェック。
            var response = _validateWhenLayOff(playerId, targetPlayerId, ReadType.Slave, (playerStatus, targetPlayerStatus, guild, error) =>
            {
                return new LayOffResponse { Error = error };
            });
            if (response.Error != null) return response;

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenLayOff(playerId, targetPlayerId, ReadType.Lock, (playerStatus, targetPlayerStatus, guild, error) =>
                {
                    if (error != null) return new LayOffResponse { Error = error };

                    // 対象プレイヤー脱退。
                    if (guild.DropOutAsNotLeader(targetPlayerStatus) && !targetPlayerStatus.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save", playerStatus.PlayerID, targetPlayerStatus.PlayerID, guild.ID));
                    }
                    tx.Commit();
                    return new LayOffResponse { TargetPlayer = targetPlayerStatus.ToOtherResponseData() };
                });
            });
        }
        static T _validateWhenLayOff<T>(uint playerId, uint targetPlayerId, ReadType readType, Func<PlayerStatusEntity, PlayerStatusEntity, GuildEntity, GuildError, T> func)
        {
            return _validateLeaderWithTargetPlayer(playerId, targetPlayerId, readType, (playerStatus, targetPlayerStatus, guild, error) =>
            {
                if (!guild.CanDropOut(targetPlayerStatus))
                {
                    throw new InvalidOperationException(string.Join("\t", "Cannot Drop Out", playerStatus.PlayerID, targetPlayerStatus.PlayerID, guild.ID));
                }
                return func.Invoke(playerStatus, targetPlayerStatus, guild, error);
            });
        }

        static T _validateLeader<T>(uint playerId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, GuildError, T> func)
        {
            return _validateInGuild(playerId, readType, (playerStatus, guild, error) =>
            {
                if (error != null) func(playerStatus, guild, error);

                // リーダーチェック。
                _checkLeaderRole(playerStatus);

                return func(playerStatus, guild, null);
            });
        }
        static T _validateLeaderWithTargetPlayer<T>(uint playerId, uint targetPlayerId, ReadType readType, Func<PlayerStatusEntity, PlayerStatusEntity, GuildEntity, GuildError, T> func)
        {
            return _validateWithOtherPlayerInSameGuild(playerId, targetPlayerId, readType, (playerStatus, targetPlayerStatus, guild, error) =>
            {
                if (error != null) return func(playerStatus, targetPlayerStatus, guild, error);

                // リーダーチェック。
                _checkLeaderRole(playerStatus);

                return func(playerStatus, targetPlayerStatus, guild, error);
            });
        }
        static void _checkLeaderRole(PlayerStatusEntity playerStatus)
        {
            if (!playerStatus.GuildRole.HasValue || playerStatus.GuildRole.Value != GuildRole.Leader)
            {
                throw new InvalidOperationException(string.Join("\t", "Is Not Leader", playerStatus.PlayerID, playerStatus.GuildID.Value));
            }
        }
    }
}
