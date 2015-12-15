using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Item;
using Noroshi.Core.Game.Possession;

namespace Noroshi.Server.Entity.Possession
{
    public class DrugHandler : AbstractPossessionHandler<Drug>
    {
        const uint MaxNum = 999;

        Dictionary<uint, PlayerItemEntity> _itemIdToPlayerItemMap;
        Dictionary<uint, DrugEntity> _drugMap;

        public DrugHandler(uint playerId, IEnumerable<uint> candidateItemIds) : base(playerId, candidateItemIds)
        {
        }

        public override PossessionCategory PossessionCategory => PossessionCategory.Drug;

        protected override void _load(IEnumerable<uint> candidateIds, bool beforeUpdate)
        {
            // ギャップロックを避けるために保有数 0 のレコードを作成してロックを掛けてしまう。
            var drugIds = candidateIds as uint[] ?? candidateIds.ToArray();
            var playerItems = beforeUpdate ? PlayerItemEntity.CreateOrReadAndBuildMulti(PlayerID, drugIds) : PlayerItemEntity.ReadAndBuildMultiByPlayerIDAndItemIDs(PlayerID, drugIds);
            _drugMap = DrugEntity.ReadAndBuildMulti(drugIds).ToDictionary(drug => drug.DrugID);
            _itemIdToPlayerItemMap = playerItems.ToDictionary(pi => pi.ItemID);
        }

        public override IPossessionObject GetPossessionObject(uint drugId, uint num)
        {
            return _drugMap.ContainsKey(drugId) ? new Drug(_drugMap[drugId], num, GetCurrentNum(drugId)) : null;
        }

        public override uint GetCurrentNum(uint drugId)
        {
            return _itemIdToPlayerItemMap.ContainsKey(drugId) ? _itemIdToPlayerItemMap[drugId].PossessionsCount : 0;
        }
        public override uint GetMaxNum(uint drugId)
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
