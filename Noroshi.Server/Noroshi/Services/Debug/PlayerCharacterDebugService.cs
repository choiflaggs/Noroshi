using System;
using System.Linq;
using Noroshi.Core.Game.Character;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services.Debug
{
    public class PlayerCharacterDebugService
    {
        public static Core.WebApi.Response.PlayerCharacter RemoveEquip(uint playerId, uint playerCharacterId)
        {
            PlayerCharacterEntity playerCharacter = null;
            ContextContainer.ShardTransaction(tx =>
            {
                playerCharacter = PlayerCharacterEntity.ReadAndBuild(playerCharacterId, ReadType.Lock);
                if (playerCharacter == null || playerCharacter.PlayerID != playerId) {
                    throw new InvalidOperationException();
                }
                foreach (var index in Enumerable.Range(Constant.FIRST_GEAR_EQUIP_POSITION, Constant.LAST_GEAR_EQUIP_POSITION)) {
                    playerCharacter.ChangeGear((byte)index, null);
                }
                playerCharacter.Save();
                tx.Commit();
            });
            return playerCharacter.ToResponseData();
        }

        public static Core.WebApi.Response.PlayerCharacter AllEquip(uint playerId, uint playerCharacterId)
        {
            PlayerCharacterEntity playerCharacter = null;
            ContextContainer.ShardTransaction(tx =>
            {
                playerCharacter = PlayerCharacterEntity.ReadAndBuild(playerCharacterId, ReadType.Lock);
                if (playerCharacter == null || playerCharacter.PlayerID != playerId) {
                    throw new InvalidOperationException();
                }
                var characterGear = CharacterGearEntity.ReadAndBuild(playerCharacterId, playerCharacter.PromotionLevel);
                var gears = characterGear.GearIDs;
                PlayerCharacterGearEntity playerCharacterGear;
                Enumerable.Range(Constant.FIRST_GEAR_EQUIP_POSITION, Constant.LAST_GEAR_EQUIP_POSITION).ToList().ForEach(index =>
                {
                    if (playerCharacter.ToResponseData().PlayerCharacterGears.Any(gear => gear.GearPosition == index && gear.GearID != 0))
                        return;
                    playerCharacterGear = null;
                    playerCharacterGear = PlayerCharacterGearEntity.Create(playerCharacter.ID, gears[index], playerCharacter.PromotionLevel, (byte)index);
                    if (playerCharacterGear != null)
                    {
                        playerCharacterGear.Save();

                    } else
                    {
                        playerCharacterGear = PlayerCharacterGearEntity.ReadAndBuild(playerCharacter.ID, playerCharacter.PromotionLevel, (byte)index);
                    }
                    playerCharacter.ChangeGear((byte)index, playerCharacterGear);
                });
                playerCharacter.Save();
                tx.Commit();
            });
            return playerCharacter.ToResponseData();
        }

        public static Core.WebApi.Response.PlayerCharacter ChangeLevel(uint playerId, uint playerCharacterId, ushort level)
        {
            var exp = CharacterLevelEntity.GetNecessaryExp(level);
            PlayerCharacterEntity playerCharacter = null;

            ContextContainer.ShardTransaction(tx =>
            {
                var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                playerCharacter = PlayerCharacterEntity.ReadAndBuild(playerCharacterId, ReadType.Lock);
                if (playerStatus == null || playerCharacter == null || playerCharacter.PlayerID != playerId) {
                    throw new InvalidOperationException();
                }
                if (!playerCharacter.ForceSetExp(exp, playerStatus.Level))
                    return;
                playerCharacter.Save();
                tx.Commit();
            });
            return playerCharacter.ToResponseData();
        }

        public static Core.WebApi.Response.PlayerCharacter ChangePromotionLevel(uint playerId, uint playerCharacterId, byte level)
        {
            PlayerCharacterEntity playerCharacter = null;
            ContextContainer.ShardTransaction(tx =>
            {
                playerCharacter = PlayerCharacterEntity.ReadAndBuild(playerCharacterId, ReadType.Lock);
                var characterGearEntity = CharacterGearEntity.ReadAndBuildByCharacterID(playerCharacterId)
                .OrderByDescending(d => d.PromotionLevel).FirstOrDefault();
                if (characterGearEntity == null)
                    return;
                var maxLevel = characterGearEntity.PromotionLevel;
                playerCharacter.ChangePromotionLevel(level, maxLevel);
                playerCharacter.Save();
                tx.Commit();
            });
            return playerCharacter.ToResponseData();
        }

        public static Core.WebApi.Response.PlayerCharacter ChangeEvolutionLevel(uint playerId, uint playerCharacterId, byte level)
        {
            PlayerCharacterEntity playerCharacter = null;
            ContextContainer.ShardTransaction(tx =>
            {
                playerCharacter = PlayerCharacterEntity.ReadAndBuild(playerCharacterId, ReadType.Lock);
                if (playerCharacter == null || playerCharacter.PlayerID != playerId) {
                    throw new InvalidOperationException();
                }
                var character = CharacterEntity.ReadAndBuild(playerCharacter.CharacterID);
                if (character == null || level == 0) {
                    throw new InvalidOperationException();
                }
                var characterEvolution = CharacterEvolutionTypeEntity.ReadAndBuildByType(character.EvolutionType)
                    .OrderByDescending(e => e.EvolutionLevel).FirstOrDefault();
                if (characterEvolution != null) {
                    var maxLevel = characterEvolution.EvolutionLevel;
                    playerCharacter.ChangeEvolutionLevel(level, maxLevel);
                }
                playerCharacter.Save();
                tx.Commit();
            });
            return playerCharacter.ToResponseData();
        }

        public static Core.WebApi.Response.PlayerCharacter ChangeActionLevel(uint playerId, uint playerCcharacterId, ushort level, ushort index)
        {
            PlayerCharacterEntity playerCharacter = null;
            ContextContainer.ShardTransaction(tx =>
            {
                playerCharacter = PlayerCharacterEntity.ReadAndBuild(playerCcharacterId, ReadType.Lock);
                if (playerCharacter == null || playerCharacter.PlayerID == playerId) {
                    throw new InvalidOperationException();
                }
                playerCharacter.ChangeActionLevel(index, level);
                playerCharacter.Save();
                tx.Commit();
            });
            return playerCharacter.ToResponseData();
        }
    }
}