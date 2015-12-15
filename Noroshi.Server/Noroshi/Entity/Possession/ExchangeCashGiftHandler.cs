using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Item;
using Noroshi.Core.Game.Possession;

namespace Noroshi.Server.Entity.Possession
{
    public class ExchangeCashGiftHandler : AbstractPossessionHandler<ExchangeCashGift>
    {
        const uint MaxNum = 999;

        Dictionary<uint, PlayerItemEntity> _itemIdToPlayerItemMap;
        Dictionary<uint, ExchangeCashGiftEntity> _exchangeCashGiftMap;

        public ExchangeCashGiftHandler(uint playerId, IEnumerable<uint> candidateItemIds) : base(playerId, candidateItemIds)
        {
        }

        public override PossessionCategory PossessionCategory => PossessionCategory.ExchangeCashGift;

        protected override void _load(IEnumerable<uint> candidateIds, bool beforeUpdate)
        {
            // ギャップロックを避けるために保有数 0 のレコードを作成してロックを掛けてしまう。
            var exchangeCashGiftIds = candidateIds as uint[] ?? candidateIds.ToArray();
            var playerItems = beforeUpdate ? PlayerItemEntity.CreateOrReadAndBuildMulti(PlayerID, exchangeCashGiftIds) : PlayerItemEntity.ReadAndBuildMultiByPlayerIDAndItemIDs(PlayerID, exchangeCashGiftIds);
            _exchangeCashGiftMap = ExchangeCashGiftEntity.ReadAndBuildMulti(exchangeCashGiftIds).ToDictionary(exchangeCashGift => exchangeCashGift.ExchangeCashGiftID);
            _itemIdToPlayerItemMap = playerItems.ToDictionary(pi => pi.ItemID);
        }

        public override IPossessionObject GetPossessionObject(uint exchangeCashGiftId, uint num)
        {
            return _exchangeCashGiftMap.ContainsKey(exchangeCashGiftId) ? new ExchangeCashGift(_exchangeCashGiftMap[exchangeCashGiftId], num, GetCurrentNum(exchangeCashGiftId)) : null;
        }

        public override uint GetCurrentNum(uint exchangeCashGiftId)
        {
            return _itemIdToPlayerItemMap.ContainsKey(exchangeCashGiftId) ? _itemIdToPlayerItemMap[exchangeCashGiftId].PossessionsCount : 0;
        }
        public override uint GetMaxNum(uint exchangeCashGiftId)
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
