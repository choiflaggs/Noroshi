using System;
using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Core.Game.Player;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Item;
using Noroshi.Server.Entity.Possession;
using Drug = Noroshi.Core.WebApi.Response.Drug;

namespace Noroshi.Server.Services.Item
{
    public class DrugService
    {
        public static PlayerDrug[] GetAll(uint playerId)
        {
            return PlayerDrugEntity.ReadAndBuildAll(playerId).Select(data => data.ToResponseData()).ToArray();
        }

        public static Drug[] MasterData()
        {
            return DrugEntity.ReadAndBuildAll().Select(data => data.ToResponseData()).ToArray();
        }

        public static CharacterExpDopingResponse ConsumeDrug(uint playerId, uint playerCharacterId, uint drugId, ushort consumingNum)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);
            if (playerStatus == null)
            {
                throw new SystemException(string.Join("\t", "Player Status Not Found", playerId));
            }
            var playerCharacter = PlayerCharacterEntity.ReadAndBuild(playerCharacterId);
            if (playerCharacter == null)
            {
                throw new InvalidOperationException(string.Join("\t", "Player Character Not Found", playerId, playerCharacterId));
            }
            var drug = DrugEntity.ReadAndBuild(drugId);
            if (drug == null)
            {
                throw new InvalidOperationException(string.Join("\t", "Drug Not Found", playerId, drugId));
            }

            var possessionManager = new PossessionManager(playerId, drug.GetPosssesionParam(consumingNum));
            possessionManager.Load();
            if (!possessionManager.CanRemove(drug.GetPosssesionParam(consumingNum)))
            {
                throw new InvalidOperationException(string.Join("\t", "Cannot Remove", playerId, playerCharacterId, drugId, consumingNum));
            }

            ContextContainer.ShardTransaction(tx =>
            {
                // ロックを掛けて取り直す。
                playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                playerCharacter = PlayerCharacterEntity.ReadAndBuild(playerCharacterId, ReadType.Lock);

                var addedExp = playerCharacter.AddExp(drug.CharacterExp, playerStatus.Level);
                if (addedExp > 0)
                {
                    // プレイヤーキャラクター経験値変動分保存。
                    if (!playerCharacter.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save", playerId, playerCharacter.ID));
                    }
                    // チュートリアル進捗。
                    var updatedTutorialStep = playerStatus.TryToProceedTutorialStep(TutorialStep.ConsumeDrug);
                    if (updatedTutorialStep.HasValue && !playerStatus.Save())
                    {
                        throw new SystemException(string.Join("\t", "Fail to Save", playerId));
                    }
                    // 支払い。
                    possessionManager.Remove(drug.GetPosssesionParam(consumingNum));

                    tx.Commit();
                }
            });
            return new CharacterExpDopingResponse
            {
                UsedDrug = possessionManager.GetPossessionObject(drug.GetPosssesionParam(consumingNum)).ToResponseData(),
                PlayerCharacter = playerCharacter.ToResponseData()
            };
        }
    }
}