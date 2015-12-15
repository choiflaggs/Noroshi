using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Item;
using Noroshi.Server.Entity.Possession;

namespace Noroshi.Server.Services.Item
{
    public class GearEnchantService
    {
        public static GearEnchantAddExpResponse AddExp(uint playerId, uint playerCharacterGearId, IEnumerable<uint> gearIds, IEnumerable<uint> gearEnchantMaterialIds, IEnumerable<uint> gearPieceIds)
        {
            PlayerCharacterGearEntity playerGear = null;
            var usingGoldPossession = new PossessionParam();
            PossessionManager possessionManager = null;

            var gearPosessions = (gearIds.Count() == 0)
                ? GearEntity.ReadAndBuildMulti(gearIds).Select(g => g.GetPosssesionParam(1))
                : Enumerable.Empty<PossessionParam>();
            var gearPiecePosessions = (gearPieceIds.Count() == 0)
                ? GearPieceEntity.ReadAndBuildMulti(gearPieceIds).Select(gearPiece => gearPiece.GetPosssesionParam(1))
                : Enumerable.Empty<PossessionParam>();
            var gearEnchantMaterialPosessions = (gearEnchantMaterialIds.Count() == 0)
                ? GearEnchantMaterialEntity.ReadAndBuildMulti(gearEnchantMaterialIds).Select((gearPiece => gearPiece.GetPosssesionParam(1)))
                : Enumerable.Empty<PossessionParam>();
            var gearEnchantExp = (uint)GearEnchantExpEntity.ReadAndBuildMulti(gearIds.Concat(gearEnchantMaterialIds).Concat(gearPieceIds))
                .Sum(enchantExp => enchantExp.EnchantExp);
            var itemParams = new List<PossessionParam>();
            if (gearPosessions.Count() > 0)
                itemParams.AddRange(gearPosessions);
            if (gearPiecePosessions.Count() > 0)
                itemParams.AddRange(gearPiecePosessions);
            if (gearEnchantMaterialPosessions.Count() > 0)
                itemParams.AddRange(gearEnchantMaterialPosessions);
            var allParams = itemParams.ToList();

            ContextContainer.ShardTransaction(tx =>
            {
                playerGear = PlayerCharacterGearEntity.ReadAndBuild(playerCharacterGearId, ReadType.Lock);
                var level = GearEntity.ReadAndBuild(playerGear.GearID).GetLevel(playerGear.Exp + gearEnchantExp);
                var needGold =
                    (uint)
                        GearEnchantLevelEntity.ReadAndBuildByGearID(playerGear.GearID)
                            .Where(enchantLevel => level >= playerGear.PromotionLevel)
                            .Sum(enchantLevel => enchantLevel.Gold);
                usingGoldPossession = PossessionManager.GetGoldParam(needGold);
                allParams.Add(usingGoldPossession);

                possessionManager = new PossessionManager(playerId, allParams);
                possessionManager.Load();
                if (!possessionManager.CanRemove(allParams)) {
                    throw new InvalidOperationException(string.Join("\t", "Item or Gold Can Not Use", playerId, gearIds,
                        gearEnchantMaterialIds, gearPieceIds, needGold));
                }
                possessionManager.Remove(allParams);
                playerGear.AddExp(gearEnchantExp);
                var result = playerGear.Save();
                if (!result)
                {
                    throw new SystemException();
                }
                tx.Commit();
            });

            return new GearEnchantAddExpResponse
            {
                Gear = playerGear.ToResponseData(),
                UseItemObjects = possessionManager.GetPossessionObjects(itemParams).Select(data => data.ToResponseData()).ToArray(),
                UseGoldObjects = possessionManager.GetPossessionObject(usingGoldPossession).ToResponseData()
            };
        }

        public static GearEnchantLevel[] LevelMasterData()
        {
            return GearEnchantLevelEntity.ReadAndBuildAll().Select(data => data.ToResponseData()).ToArray();
        }

        public static GearEnchantExp[] ExpMasterData()
        {
            return GearEnchantExpEntity.ReadAndBuildAll().Select(data => data.ToResponseData()).ToArray();
        }

    }
}