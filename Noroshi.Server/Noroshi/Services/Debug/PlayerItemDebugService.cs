using System;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb.Item;
using Noroshi.Server.Entity.Item;
using Noroshi.Server.Entity.Player;

namespace Noroshi.Server.Services.Debug
{
    public class PlayerItemDebugService
    {
        public static PlayerItem GetItem(uint playerId, uint itemId, ushort addPossessionsCount)
        {
            //if (ItemEntity.ReadAndBuild(new ItemDao.PrimaryKey {ID = itemId}) == null)
            //{
            //    throw new NullReferenceException("ItemData Not Found:" + itemId);
            //}
            if (addPossessionsCount <= 0) {
                throw new InvalidOperationException();
            }
            var queryPlayerItemData = ContextContainer.NoroshiTransaction(tx =>
            {
                var playerItemData = PlayerItemEntity.CreateOrReadAndBuild(playerId, itemId);
                playerItemData.SetNum(addPossessionsCount + playerItemData.PossessionsCount);
                playerItemData.Save();
                tx.Commit();
                return playerItemData;
            });

            return queryPlayerItemData.ToResponseData();
        }

        public static PlayerItem UseItem(uint playerId, uint itemId, ushort usePossessionsCount)
        {
            //if (ItemEntity.ReadAndBuild(new ItemDao.PrimaryKey { ID = itemId }) == null) {
            //    throw new NullReferenceException("ItemData Not Found:" + itemId);
            //}
            if (usePossessionsCount <= 0) {
                throw new InvalidOperationException();
            }
            var queryPlayerItemData = ContextContainer.NoroshiTransaction(tx =>
            {
                var playerItemData = PlayerItemEntity.CreateOrReadAndBuild(playerId,
                    itemId);
                if (playerItemData.PossessionsCount < usePossessionsCount)
                {
                    throw new InvalidCastException();
                }
                playerItemData.SetNum(playerItemData.PossessionsCount - usePossessionsCount);
                playerItemData.Save();
                tx.Commit();
                return playerItemData;
            });

            return queryPlayerItemData.ToResponseData();
        }

    }
}