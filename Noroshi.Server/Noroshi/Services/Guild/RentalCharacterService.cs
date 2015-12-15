using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response.Guild;
using Noroshi.Core.Game.Guild;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services.Guild
{
    public class RentalCharacterService : AbstractGuildService
    {
        /// <summary>
        /// バトルに連れていくことが可能な傭兵キャラクター取得。
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <returns></returns>
        public static GetTakableRentalCharactersResponse GetTakableRentalCharacters(uint playerId)
        {
            return _validateInGuild(playerId, ReadType.Slave, (playerStatus, guild, error) =>
            {
                if (error != null) return _getBlankGetRentalCharactersResponse();

                // 初心者ギルドは傭兵利用不可。
                if (guild.Category == GuildCategory.Beginner) return _getBlankGetRentalCharactersResponse();

                // 他ギルドメンバーを取得し、
                var guildMemberPlayerIds = guild.GetMemberPlayerStatuses().Select(ps => ps.PlayerID).Where(pid => pid != playerId);
                if (guildMemberPlayerIds.Count() == 0)
                {
                    return _getBlankGetRentalCharactersResponse();
                }
                // 派遣されている傭兵キャラクターを取得。
                var rentalPlayerCharacterIds = PlayerRentalCharacterEntity.ReadAndBuildActiveByPlayerIDs(guildMemberPlayerIds).Select(prc => prc.PlayerCharacterID);
                if (rentalPlayerCharacterIds.Count() == 0)
                {
                    return _getBlankGetRentalCharactersResponse();
                }

                // プレイヤーキャラクターをビルド。
                // ただし、対象プレイヤーのレベルを超えていないプレイヤーキャラクターに限る。
                var availablePlayerCharacters = PlayerCharacterEntity.ReadAndBuildMulti(rentalPlayerCharacterIds).Where(pc => pc.Level <= playerStatus.Level);

                return new GetTakableRentalCharactersResponse
                {
                    RentalPlayerCharacters = availablePlayerCharacters.Select(pc => pc.ToResponseData()).ToArray(),
                };
            });
        }
        static GetTakableRentalCharactersResponse _getBlankGetRentalCharactersResponse()
        {
            return new GetTakableRentalCharactersResponse { RentalPlayerCharacters = new Core.WebApi.Response.PlayerCharacter[0] };
        }

        /// <summary>
        /// 派遣中の傭兵キャラクター取得。
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public static GetRentalCharactersResponse GetRentalCharacters(uint playerId)
        {
            return _validateInGuild(playerId, ReadType.Slave, (playerStatus, guild, error) =>
            {
                if (error != null) return new GetRentalCharactersResponse { Error = error };

                // 初心者ギルドは傭兵利用不可。
                if (guild.Category == GuildCategory.Beginner)
                {
                    throw new InvalidOperationException(string.Join("\t", "Is Beginner Guild Member", playerId, guild.ID));
                }

                // 派遣中傭兵キャラクターを取得。
                var playerRentalCharacters = PlayerRentalCharacterEntity.ReadAndBuildActiveByPlayerID(playerId);

                // プレイヤーキャラクターをビルド。
                var playerCharacterMap = playerRentalCharacters.Count() > 0
                    ? PlayerCharacterEntity.ReadAndBuildMulti(playerRentalCharacters.Select(prc => prc.PlayerCharacterID)).ToDictionary(pc => pc.ID)
                    : new Dictionary<uint, PlayerCharacterEntity>();

                return new GetRentalCharactersResponse
                {
                    RentalCharacters = playerRentalCharacters.Select(prc => prc.ToResponseData(playerCharacterMap[prc.PlayerCharacterID])).ToArray(),
                };
            });
        }

        /// <summary>
        /// 傭兵を派遣する。
        /// </summary>
        /// <param name="playerId">派遣主プレイヤーID</param>
        /// <param name="no">傭兵番号</param>
        /// <param name="playerCharacterId">派遣されるプレイヤーキャラクターID</param>
        /// <returns></returns>
        public static AddRentalCharacterResponse AddRentalCharacter(uint playerId, byte no, uint playerCharacterId)
        {
            // SLAVE チェック。
            var response = _validateWhenAddRentalCharacter(playerId, no, playerCharacterId, ReadType.Slave, (playerStatus, guild, playerCharacter, error) =>
            {
                if (error != null) return new AddRentalCharacterResponse { Error = error };

                // 対象傭兵番号が既に埋まっていないかチェック。
                var playerRentalCharacter = PlayerRentalCharacterEntity.ReadAndBuild(playerId, no);
                if (playerRentalCharacter != null)
                {
                    throw new InvalidOperationException(string.Join("\t", "Duplicate No", playerId, no, playerCharacterId, playerRentalCharacter.PlayerCharacterID));
                }
                // 正常時。
                return new AddRentalCharacterResponse();
            });
            if (response.Error != null) return response;

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenAddRentalCharacter(playerId, no, playerCharacterId, ReadType.Lock, (playerStatus, guild, playerCharacter, error) =>
                {
                    if (error != null) return new AddRentalCharacterResponse { Error = error };

                    // 傭兵登録実行。
                    var playerRentalCharacter = PlayerRentalCharacterEntity.Create(playerStatus, no, playerCharacter);
                    if (playerRentalCharacter == null)
                    {
                        throw new InvalidOperationException(string.Join("\t", "Duplicate No", playerId, no, playerCharacterId));
                    }

                    tx.Commit();

                    return new AddRentalCharacterResponse
                    {
                        PlayerCharacter = playerCharacter.ToResponseData(),
                    };
                });
            });
        }
        static T _validateWhenAddRentalCharacter<T>(uint playerId, byte no, uint playerCharacterId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, PlayerCharacterEntity, GuildError, T> func)
        {
            return _validateInGuild(playerId, readType, (playerStatus, guild, error) =>
            {
                if (error != null) func(playerStatus, guild, null, error);

                // 該当プレイヤーキャラクターをビルド。
                var playerCharacter = PlayerCharacterEntity.ReadAndBuild(playerCharacterId, readType);
                if (playerCharacter == null)
                {
                    throw new InvalidOperationException(string.Join("\t", "Player Character Not Found", playerId, playerCharacterId));
                }
                // 初心者ギルドは禁止。
                if (guild.Category == GuildCategory.Beginner)
                {
                    throw new InvalidOperationException(string.Join("\t", "Beginner Guild Member Cannot Create", playerId, guild.ID));
                }
                // 傭兵派遣可否チェック。
                if (!PlayerRentalCharacterEntity.CanCreate(playerStatus, no, playerCharacter))
                {
                    throw new InvalidOperationException(string.Join("\t", "Cannot Create", playerStatus.PlayerID, no, playerCharacter.ID));
                }

                return func(playerStatus, guild, playerCharacter, null);
            });
        }

        /// <summary>
        /// 傭兵を帰還させる。
        /// </summary>
        /// <param name="playerId">派遣主プレイヤーID</param>
        /// <returns></returns>
        public static RemoveRentalCharacterResponse RemoveRentalCharacters(uint playerId)
        {
            // SLAVE チェック。
            var response = _validateWhenRemoveRentalCharacters(playerId, ReadType.Slave, (playerStatus, guild, rentalCharacter, playerCharacter, error) =>
            {
                return new RemoveRentalCharacterResponse { Error = error };
            });
            if (response.Error != null) return response;

            // トランザクション。
            return ContextContainer.ShardTransaction(tx =>
            {
                return _validateWhenRemoveRentalCharacters(playerId, ReadType.Lock, (playerStatus, guild, playerRentalCharacters, playerCharacters, error) =>
                {
                    if (error != null) return new RemoveRentalCharacterResponse { Error = error };

                    // 報酬取得。
                    var playerCharacterMap = playerCharacters.ToDictionary(pc => pc.ID);
                    var fixedRewards = playerRentalCharacters.SelectMany(prc => prc.GetFixedRewards(playerCharacterMap[prc.PlayerCharacterID].Level));
                    var rentalRewards = playerRentalCharacters.SelectMany(prc => prc.GetRentalRewards(playerStatus, playerCharacterMap[prc.PlayerCharacterID].Level));
                    var allRewards = playerRentalCharacters.SelectMany(prc => prc.GetAllRewards(playerStatus, playerCharacterMap[prc.PlayerCharacterID].Level));

                    // 派遣解除
                    if (!playerRentalCharacters.All(prc => prc.Remove()))
                    {
                        throw new SystemException(string.Join("\t", "Fail to Update"));
                    }

                    // Possession 付与。
                    var possessionManager = new PossessionManager(playerId, playerRentalCharacters.SelectMany(prc => prc.GetAllRewards(playerStatus, playerCharacterMap[prc.PlayerCharacterID].Level)));
                    possessionManager.Add(allRewards);

                    tx.Commit();

                    return new RemoveRentalCharacterResponse
                    {
                        PlayerCharacters = playerCharacters.Select(pc => pc.ToResponseData()).ToArray(),
                        FixedRewards = possessionManager.GetPossessionObjects(fixedRewards).Select(po => po.ToResponseData()).ToArray(),
                        RentalRewards = possessionManager.GetPossessionObjects(rentalRewards).Select(po => po.ToResponseData()).ToArray(),
                    };
                });
            });
        }
        static T _validateWhenRemoveRentalCharacters<T>(uint playerId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, IEnumerable<PlayerRentalCharacterEntity>, IEnumerable<PlayerCharacterEntity>, GuildError, T> func)
        {
            return _validateInGuild(playerId, readType, (playerStatus, guild, error) =>
            {
                if (error != null) func(playerStatus, guild, null, null, error);

                // 対象プレイヤーの傭兵情報をビルド。
                var playerRentalCharacters = PlayerRentalCharacterEntity.ReadAndBuildByPlayerStatus(playerStatus, readType);
                if (playerRentalCharacters.Count() == 0 || playerRentalCharacters.All(prc => !prc.CanRemove()))
                {
                    return func(playerStatus, guild, playerRentalCharacters, null, new GuildError { NoTarget = true });
                }
                // 該当プレイヤーキャラクターをビルド。
                var playerCharacters = PlayerCharacterEntity.ReadAndBuildMulti(playerRentalCharacters.Select(pc => pc.PlayerCharacterID), readType);

                return func(playerStatus, guild, playerRentalCharacters, playerCharacters, null);
            });
        }
    }
}
