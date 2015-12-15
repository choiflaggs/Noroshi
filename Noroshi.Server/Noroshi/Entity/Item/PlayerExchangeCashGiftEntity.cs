using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;

namespace Noroshi.Server.Entity.Item
{
    public class PlayerExchangeCashGiftEntity : AbstractDaoWrapperEntity<PlayerExchangeCashGiftEntity, PlayerItemDao, PlayerItemDao.PrimaryKey, PlayerItemDao.Record>
    {
        public static IEnumerable<PlayerExchangeCashGiftEntity> ReadAndBuildMulti(uint playerId, IEnumerable<uint> exchangeCashGiftIds, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(exchangeCashGiftIds.Select(id => new PlayerItemDao.PrimaryKey { PlayerID = playerId, ItemID = id }), readType);
        }
        public static IEnumerable<PlayerExchangeCashGiftEntity> ReadAndBuildAll(uint playerId)
        {
            // TODO : PlayerItem にカラム追加不要かどうかは検討。
            var exchangeCashGiftEntities = ExchangeCashGiftEntity.ReadAndBuildAll();
            return ReadAndBuildMulti(playerId, exchangeCashGiftEntities.Select(exchangeCashGift => exchangeCashGift.ExchangeCashGiftID));
        }

        public uint ExchangeCashGiftID => _record.ItemID;
        public uint PossessionsCount => _record.PossessionsCount;

        public Core.WebApi.Response.PlayerExchangeCashGift ToResponseData()
        {
            return new Core.WebApi.Response.PlayerExchangeCashGift
            {
                ExchangeCashGiftID = ExchangeCashGiftID,
                PossessionsCount = PossessionsCount
            };
        }
    }
}
