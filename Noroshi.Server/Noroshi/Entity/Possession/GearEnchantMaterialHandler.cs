using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Item;
using Noroshi.Core.Game.Possession;

namespace Noroshi.Server.Entity.Possession
{
    public class GearEnchantMaterialHandler : AbstractPossessionHandler<GearEnchantMaterial>
    {
        const uint MaxNum = 999;

        Dictionary<uint, PlayerItemEntity> _itemIdToPlayerItemMap;
        Dictionary<uint, GearEnchantMaterialEntity> _gearEnchantMaterialMap;

        public GearEnchantMaterialHandler(uint playerId, IEnumerable<uint> candidateItemIds) : base(playerId, candidateItemIds)
        {
        }

        public override PossessionCategory PossessionCategory => PossessionCategory.GearEnchantMaterial;

        protected override void _load(IEnumerable<uint> candidateIds, bool beforeUpdate)
        {
            // ギャップロックを避けるために保有数 0 のレコードを作成してロックを掛けてしまう。
            var gearEnchantMaterialIds = candidateIds as uint[] ?? candidateIds.ToArray();
            var playerItems = beforeUpdate ? PlayerItemEntity.CreateOrReadAndBuildMulti(PlayerID, gearEnchantMaterialIds) : PlayerItemEntity.ReadAndBuildMultiByPlayerIDAndItemIDs(PlayerID, gearEnchantMaterialIds);
            _gearEnchantMaterialMap = GearEnchantMaterialEntity.ReadAndBuildMulti(gearEnchantMaterialIds).ToDictionary(gearEnchantMaterial => gearEnchantMaterial.GearEnchantMaterialID);
            _itemIdToPlayerItemMap = playerItems.ToDictionary(pi => pi.ItemID);
        }

        public override IPossessionObject GetPossessionObject(uint gearEnchantMaterialId, uint num)
        {
            return _gearEnchantMaterialMap.ContainsKey(gearEnchantMaterialId) ? new GearEnchantMaterial(_gearEnchantMaterialMap[gearEnchantMaterialId], num, GetCurrentNum(gearEnchantMaterialId)) : null;
        }

        public override uint GetCurrentNum(uint gearEnchantMaterialId)
        {
            return _itemIdToPlayerItemMap.ContainsKey(gearEnchantMaterialId) ? _itemIdToPlayerItemMap[gearEnchantMaterialId].PossessionsCount : 0;
        }
        public override uint GetMaxNum(uint gearEnchantMaterialId)
        {
            return MaxNum;
        }

        protected override bool _add(IEnumerable<IPossessionParam> possessionParams)
        {
            foreach (var possessionParam in possessionParams) {
                var playerItem = _itemIdToPlayerItemMap[possessionParam.ID];
                playerItem.SetNum(playerItem.PossessionsCount + possessionParam.Num);
                playerItem.Save();
            }
            return true;
        }

        protected override bool _remove(IEnumerable<IPossessionParam> possessionParams)
        {
            foreach (var possessionParam in possessionParams) {
                var playerItem = _itemIdToPlayerItemMap[possessionParam.ID];
                playerItem.SetNum(playerItem.PossessionsCount - possessionParam.Num);
                playerItem.Save();
            }
            return true;
        }
    }
}
