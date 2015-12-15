using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Item;
using Noroshi.Core.Game.Possession;

namespace Noroshi.Server.Entity.Possession
{
    public class RaidTicketHandler : AbstractPossessionHandler<RaidTicket>
    {
        const uint MaxNum = 999;

        Dictionary<uint, PlayerItemEntity> _itemIdToPlayerItemMap;
        Dictionary<uint, RaidTicketEntity> _raidTicketMap;

        public RaidTicketHandler(uint playerId, IEnumerable<uint> candidateItemIds) : base(playerId, candidateItemIds)
        {
        }

        public override PossessionCategory PossessionCategory => PossessionCategory.RaidTicket;

        protected override void _load(IEnumerable<uint> candidateIds, bool beforeUpdate)
        {
            // ギャップロックを避けるために保有数 0 のレコードを作成してロックを掛けてしまう。
            var raidTicketIds = candidateIds as uint[] ?? candidateIds.ToArray();
            var playerItems = beforeUpdate ? PlayerItemEntity.CreateOrReadAndBuildMulti(PlayerID, raidTicketIds) : PlayerItemEntity.ReadAndBuildMultiByPlayerIDAndItemIDs(PlayerID, raidTicketIds);
            _raidTicketMap = RaidTicketEntity.ReadAndBuildMulti(raidTicketIds).ToDictionary(raidTicket => raidTicket.RaidTicketID);
            _itemIdToPlayerItemMap = playerItems.ToDictionary(pi => pi.ItemID);
        }

        public override IPossessionObject GetPossessionObject(uint raidTicketId, uint num)
        {
            return _raidTicketMap.ContainsKey(raidTicketId) ? new RaidTicket(_raidTicketMap[raidTicketId], num, GetCurrentNum(raidTicketId)) : null;
        }

        public override uint GetCurrentNum(uint raidTicketId)
        {
            return _itemIdToPlayerItemMap.ContainsKey(raidTicketId) ? _itemIdToPlayerItemMap[raidTicketId].PossessionsCount : 0;
        }
        public override uint GetMaxNum(uint raidTicketId)
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
