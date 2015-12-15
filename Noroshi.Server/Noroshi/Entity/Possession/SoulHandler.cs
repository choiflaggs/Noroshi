using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Item;
using Noroshi.Core.Game.Possession;

namespace Noroshi.Server.Entity.Possession
{
    public class SoulHandler : AbstractPossessionHandler<Soul>
    {
        const uint MaxNum = 999;

        Dictionary<uint, PlayerItemEntity> _itemIdToPlayerItemMap;
        Dictionary<uint, SoulEntity> _soulMap;
        
        public SoulHandler(uint playerId, IEnumerable<uint> candidateItemIds) :base(playerId, candidateItemIds)
        {
        }

        public override PossessionCategory PossessionCategory => PossessionCategory.Soul;

        protected override void _load(IEnumerable<uint> candidateIds, bool beforeUpdate)
        {
            // ギャップロックを避けるために保有数 0 のレコードを作成してロックを掛けてしまう。
            var soulIds = candidateIds as uint[] ?? candidateIds.ToArray();
            var playerItems = beforeUpdate ? PlayerItemEntity.CreateOrReadAndBuildMulti(PlayerID, soulIds) : PlayerItemEntity.ReadAndBuildMultiByPlayerIDAndItemIDs(PlayerID, soulIds);
            _soulMap = SoulEntity.ReadAndBuildMulti(soulIds).ToDictionary(soul => soul.SoulID);
            _itemIdToPlayerItemMap = playerItems.ToDictionary(pi => pi.ItemID);
        }

        public override IPossessionObject GetPossessionObject(uint soulId, uint num)
        {
            return _soulMap.ContainsKey(soulId) ? new Soul(_soulMap[soulId], num, GetCurrentNum(soulId)) : null;
        }

        public override uint GetCurrentNum(uint soulId)
        {
            return _itemIdToPlayerItemMap.ContainsKey(soulId) ? _itemIdToPlayerItemMap[soulId].PossessionsCount : 0;
        }
        public override uint GetMaxNum(uint soulId)
        {
            return MaxNum;
        }

        protected override bool _add(IEnumerable<IPossessionParam> possessionParams)
        {
            foreach (var possessionParam in possessionParams)
            {
                var playerItem = _itemIdToPlayerItemMap[possessionParam.ID];
                playerItem.SetNum(playerItem.PossessionsCount + possessionParam.Num);
                playerItem.Save();
            }
            return true;
        }

        protected override bool _remove(IEnumerable<IPossessionParam> possessionParams)
        {
            foreach (var possessionParam in possessionParams)
            {
                var playerItem = _itemIdToPlayerItemMap[possessionParam.ID];
                playerItem.SetNum(playerItem.PossessionsCount - possessionParam.Num);
                playerItem.Save();
            }
            return true;
        }
    }
}
