using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Item;
using Noroshi.Core.Game.Possession;

namespace Noroshi.Server.Entity.Possession
{
    public class GearPieceHandler : AbstractPossessionHandler<GearPiece>
    {
        const uint MaxNum = 999;

        Dictionary<uint, PlayerItemEntity> _itemIdToPlayerItemMap;
        Dictionary<uint, GearPieceEntity> _gearPieceMap;

        public GearPieceHandler(uint playerId, IEnumerable<uint> candidateItemIds) : base(playerId, candidateItemIds)
        {
        }

        public override PossessionCategory PossessionCategory => PossessionCategory.GearPiece;

        protected override void _load(IEnumerable<uint> candidateIds, bool beforeUpdate)
        {
            // ギャップロックを避けるために保有数 0 のレコードを作成してロックを掛けてしまう。
            var gearPieceIds = candidateIds as uint[] ?? candidateIds.ToArray();
            var playerItems = beforeUpdate ? PlayerItemEntity.CreateOrReadAndBuildMulti(PlayerID, gearPieceIds) : PlayerItemEntity.ReadAndBuildMultiByPlayerIDAndItemIDs(PlayerID, gearPieceIds);
            _gearPieceMap = GearPieceEntity.ReadAndBuildMulti(gearPieceIds).ToDictionary(gearPiece => gearPiece.GearPieceID);
            _itemIdToPlayerItemMap = playerItems.ToDictionary(pi => pi.ItemID);
        }

        public override IPossessionObject GetPossessionObject(uint gearPieceId, uint num)
        {
            return _gearPieceMap.ContainsKey(gearPieceId) ? new GearPiece(_gearPieceMap[gearPieceId], num, GetCurrentNum(gearPieceId)) : null;
        }

        public override uint GetCurrentNum(uint gearPieceId)
        {
            return _itemIdToPlayerItemMap.ContainsKey(gearPieceId) ? _itemIdToPlayerItemMap[gearPieceId].PossessionsCount : 0;
        }
        public override uint GetMaxNum(uint gearPieceId)
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
