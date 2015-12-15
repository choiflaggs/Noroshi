using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response.PresentBox;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.PresentBox;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services
{
    public class PresentBoxService
    {
        public static ListResponse List(uint playerId)
        {
            var presentBoxes = PresentBoxEntity.ReadAndBuildByPlayerID(playerId);

            var possessionParams = presentBoxes.SelectMany(pb => pb.PossessionParams);
            var possessionManager = new PossessionManager(playerId, possessionParams);
            possessionManager.Load();

            return new ListResponse
            {
                PresentBoxes = presentBoxes.Select(pb => pb.ToResponseData(possessionManager)).ToArray(),
            };
        }
        public static ReceiveResponse Receive(uint playerId, uint[] presentBoxIds)
        {
            // 該当プレゼントボックスをビルド。
            var presentBoxes = _readAndBuildPresentBoxesWithValidationToReceive(playerId, presentBoxIds, ReadType.Slave);

            var possessionParams = presentBoxes.SelectMany(pb => pb.PossessionParams);
            var possessionManager = new PossessionManager(playerId, possessionParams);

            ContextContainer.ShardTransaction(tx =>
            {
                // 該当プレゼントボックスをビルド。ロックはかける。
                presentBoxes = _readAndBuildPresentBoxesWithValidationToReceive(playerId, presentBoxIds, ReadType.Lock);

                possessionParams = presentBoxes.SelectMany(pb => pb.PossessionParams);
                possessionManager = new PossessionManager(playerId, possessionParams);

                // 受け取りログ作成。
                PresentBoxReceivedLogEntity.CreateMulti(presentBoxes);
                // 受け取り後は不要なので削除。
                foreach (var presentBox in new List<PresentBoxEntity>(presentBoxes))
                {
                    var presentBoxId = presentBox.ID;
                    if (!presentBox.Delete())
                    {
                        throw new Exception(string.Join("\t", "Cannot Delete Present Box", playerId, presentBoxId));
                    }
                }

                // Possession 付与
                possessionManager.Add(possessionParams);
            });

            return new ReceiveResponse
            {
                ReceivingPresentBoxes = presentBoxes.Select(pb => pb.ToResponseData(possessionManager)).ToArray(),
            };
        }
        static IEnumerable<PresentBoxEntity> _readAndBuildPresentBoxesWithValidationToReceive(uint playerId, IEnumerable<uint> presentBoxIds, ReadType readType)
        {
            // 該当プレゼントボックスをビルド。
            var presentBoxes = PresentBoxEntity.ReadAndBuildMulti(presentBoxIds, readType);

            // レコード有無チェック
            var invalidPresentBoxIds = presentBoxIds.Where(presentBoxId => presentBoxes.All(pb => pb.ID != presentBoxId));
            if (invalidPresentBoxIds.Count() > 0)
            {
                throw new InvalidOperationException(string.Join("\t", "Invalid Present Box ID", playerId, invalidPresentBoxIds));
            }
            // 所有者チェック
            var notOwnerPresentBoxes = presentBoxes.Where(pb => pb.PlayerID != playerId);
            if (notOwnerPresentBoxes.Count() > 0)
            {
                throw new InvalidOperationException(string.Join("\t", "Not Present Box Owner", playerId, notOwnerPresentBoxes.Select(pb => pb.ID)));
            }
            // 受け取り可否チェック
            var cannotReceivePresentBoxes = presentBoxes.Where(pb => pb.CanReceive());
            if (cannotReceivePresentBoxes.Count() > 0)
            {
                throw new InvalidOperationException(string.Join("\t", "Cannot Receive Present Box", playerId, cannotReceivePresentBoxes.Select(pb => pb.ID)));
            }
            return presentBoxes;
        }
    }
}
