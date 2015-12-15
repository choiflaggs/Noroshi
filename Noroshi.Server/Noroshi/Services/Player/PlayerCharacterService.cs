using System;
using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Core.Game.Player;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Schemas;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Item;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Possession;
using CharacterConstant = Noroshi.Core.Game.Character.Constant;
using Noroshi.Server.Entity.Quest;

namespace Noroshi.Server.Services.Player
{
    public class PlayerCharacterService
    {
        public static PlayerCharacter[] Get(uint playerId)
        {
            return PlayerCharacterEntity.ReadAndBuildMultiByPlayerID(playerId).Select(e => e.ToResponseData()).ToArray();
        }

        public static PlayerCharacter[] GetAllCharacters(uint playerId)
        {
            var playerCharacters = PlayerCharacterEntity.ReadAndBuildMultiByPlayerID(playerId).Select(e => e.ToResponseData()).ToList();
            var characters = CharacterEntity.ReadAndBuildAll();
            foreach (var character in characters)
            {
                if (playerCharacters.Any(playerCharacter => playerCharacter.CharacterID == character.ID))
                    continue;
                var emptyPlayerCharacterData = new PlayerCharacter
                {
                    ID = 0,
                    Level = 0,
                    Exp = 0,
                    CharacterID = character.ID,
                    ActionLevel1 = 0,
                    ActionLevel2 = 0,
                    ActionLevel3 = 0,
                    ActionLevel4 = 0,
                    ActionLevel5 = 0,
                    EvolutionLevel = 0,
                    GearID1 = 0,
                    GearID2 = 0,
                    GearID3 = 0,
                    GearID4 = 0,
                    GearID5 = 0,
                    GearID6 = 0,
                    PromotionLevel = 0
                };
                playerCharacters.Add(emptyPlayerCharacterData);
            }
            return playerCharacters.ToArray();
        }

        public static PlayerCharacterAndPlayerItemsResponse UpPromotionLevel(uint playerId, uint playerCharacterId)
        {
            PlayerCharacterEntity playerCharacter = null;
            PossessionManager possessionManager = null;
            var possessions = Enumerable.Empty<PossessionParam>();
            ContextContainer.ShardTransaction(tx =>
            {
                playerCharacter = PlayerCharacterEntity.ReadAndBuild(playerCharacterId, ReadType.Lock);

                if (playerCharacter == null || playerCharacter.PlayerID != playerId || !Enumerable.Range(CharacterConstant.FIRST_GEAR_EQUIP_POSITION, CharacterConstant.LAST_GEAR_EQUIP_POSITION).All(index => playerCharacter.HasGear((byte)index)))
                {
                    throw new InvalidOperationException();
                }

                var maxLevel = CharacterGearEntity.ReadAndBuildByCharacterID(playerCharacter.CharacterID)
                    .Max(d => d.PromotionLevel);

                if (maxLevel == 0)
                    return;

                // グレードを変更
                playerCharacter.ChangePromotionLevel((byte)(playerCharacter.PromotionLevel + 1), maxLevel);

                // 装備されていた装備の10%を錬成剤にして返す
                var allExp = playerCharacter.GetPlayerCharacterGear().Sum(gear => gear.Exp);
                possessions = GearEnchantMaterialEntity.ConvertToExpToEnchantMaterial((uint)(allExp / 10));
                possessionManager = new PossessionManager(playerId, possessions);
                possessionManager.Load();
                possessionManager.Add(possessions);

                // 装備を外して返す
                playerCharacter.RemoveAllEquipGear();
                var result = playerCharacter.Save();
                if (result == false)
                {
                    throw new SystemException();
                }

                // チュートリアル進捗処理。
                PlayerStatusEntity.TryToProceedTutorialStep(playerId, TutorialStep.PromotionLevelUP);
                // ミッション：特定プロモーションランクキャラの所持数.
                QuestTriggerEntity.CountUpPlayerCharacterPromotionColorNum(playerCharacter.PlayerID, playerCharacter.PromotionLevel);

                tx.Commit();
            });
            return new PlayerCharacterAndPlayerItemsResponse
            {
                PlayerCharacter = playerCharacter.ToResponseData(),
                GettingGearEnchantMaterials = possessionManager.GetPossessionObjects(possessions)
                .Select(possessionObject => possessionObject.ToResponseData()).ToArray()
            };
        }

        public static PlayerCharacterAndStatusResponse UpActionLevel(uint playerId, uint playerCharacterId, ushort level, byte index)
        {
            if (level == 0)
            {
                throw new InvalidOperationException();
            }
            PlayerCharacterEntity playerCharacter = null;
            PlayerStatusEntity playerStatus = null;

            ContextContainer.ShardTransaction(tx =>
            {
                playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                if (playerStatus == null)
                {
                    throw new NullReferenceException($"PlayerStatus Not Found: {playerId}");
                }
                if (playerStatus.ActionLevelPoint == 0)
                {
                    throw new InvalidOperationException($"Character ActionLevel ShortfallActionLevelPoint PlayerID : {playerId} ActionPoint : {playerStatus.ActionLevelPoint} PlayerCharacterID : {playerCharacterId}");
                }

                playerCharacter = PlayerCharacterEntity.ReadAndBuild(playerCharacterId, ReadType.Lock);
                if (playerCharacter == null || playerCharacter.PlayerID != playerId)
                {
                    throw new NullReferenceException("PlayerCharacterData Not Found:" + playerCharacterId);
                }
                var schema = Enumerable.Range(playerCharacter.Level, level)
                        .Select(upLevel => new ActionLevelUpPaymentSchema.PrimaryKey {Level = (ushort)upLevel});
                var useGold = (uint)ActionLevelUpPaymentEntity.ReadAndBuildMulti(schema).Sum(price => price.Gold);
                ushort count = level;
                switch (index)
                {
                    case 1:
                        level += playerCharacter.ActionLevel1;
                        break;
                    case 2:
                        level += playerCharacter.ActionLevel2;
                        break;
                    case 3:
                        level += playerCharacter.ActionLevel3;
                        break;
                    case 4:
                        level += playerCharacter.ActionLevel4;
                        break;
                    case 5:
                        level += playerCharacter.ActionLevel5;
                        break;
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }

                if (playerStatus.Gold < useGold)
                {
                    throw new AggregateException("Player's Gold is not enough:" + playerId);
                }
                if (level > playerCharacter.Level)
                {
                    throw new AggregateException($"not possible to up the action level PlayerID: {playerId} " +
                                                 $"PlayerCharacterID: {playerCharacterId} index: {index} " +
                                                 $"ActionLevel: {level} CharacterLevel: {playerCharacter.Level}");
                }
                playerCharacter.ChangeActionLevel(index, level);
                var result = playerCharacter.Save();
                playerStatus.ConsumeGold(useGold);
                playerStatus.ConsumeActionLevelPoint((byte)count);
                result = result && playerStatus.Save();
                if (result == false)
                {
                    throw new SystemException();
                }
                // チュートリアル進捗更新処理
                PlayerStatusEntity.TryToProceedTutorialStep(playerId, TutorialStep.ActionLevelUP);

                // ミッション：キャラクターのアクションレベルアップ回数.
                QuestTriggerEntity.CountUpCharacterActionLevelUpNum(playerId, count);

                tx.Commit();
            });
            return new PlayerCharacterAndStatusResponse
            {
                PlayerCharacter = playerCharacter.ToResponseData(),
                PlayerStatus = playerStatus.ToResponseData()
            };
        }

        public static PlayerCharacterChangeGearResponse EquipCharacter(uint playerId, uint playerCharacterId, uint gearId, byte index)
        {
            if (index > CharacterConstant.LAST_GEAR_EQUIP_POSITION || index < CharacterConstant.FIRST_GEAR_EQUIP_POSITION)
            {
                throw new InvalidOperationException();
            }
            PlayerCharacterEntity playerCharacter = null;
            var gearData = GearEntity.ReadAndBuild(gearId);
            PossessionManager possessionManager = null;
            ContextContainer.ShardTransaction(tx =>
            {
                playerCharacter = PlayerCharacterEntity.ReadAndBuild(playerCharacterId, ReadType.Lock);
                if (playerCharacter == null || playerCharacter.PlayerID != playerId)
                {
                    throw new InvalidOperationException();
                }

                possessionManager = new PossessionManager(playerId, gearData.GetPosssesionParam(1));
                possessionManager.Load();

                if (!possessionManager.CanRemove(gearData.GetPosssesionParam(1)))
                {
                    throw new InvalidOperationException();
                }

                var playerCharacterGear = PlayerCharacterGearEntity.Create(playerCharacter.ID, gearId, playerCharacter.PromotionLevel, index);
                if (playerCharacterGear == null)
                {
                    throw new InvalidOperationException();
                }
                playerCharacter.ChangeGear(index, playerCharacterGear);
                var result = playerCharacterGear.Save();
                result = result && playerCharacter.Save();
                if (result == false)
                {
                    throw new SystemException();
                }

                // チュートリアル進捗処理。
                PlayerStatusEntity.TryToProceedTutorialStep(playerId, TutorialStep.EquipGear);

                tx.Commit();
            });

            var response = new PlayerCharacterChangeGearResponse
            {
                PlayerCharacter = playerCharacter.ToResponseData(),
                PlayerGearObject = possessionManager.GetPossessionObject(gearData.GetPosssesionParam(1)).ToResponseData()
            };

            return response;
        }
    }
}
