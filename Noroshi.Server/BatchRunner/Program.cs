using System;
using Noroshi.Server.Contexts;
using Noroshi.Server.Services.Guild;
using PushNotification;

namespace BatchRunner
{
    class Program
    {
        enum Batch
        {
            UpdateGuildRanking,
            OptimizeGuild,
            GiveGuildRankReward,
            PushNotification,
        }

        static void Main(string[] args)
        {
            var batch = (Batch)Enum.Parse(typeof(Batch), args[0]);
            uint shardId = uint.Parse(args[1]);

            ContextContainer.Initialize(new CliContext(shardId));

            switch (batch)
            {
                case Batch.UpdateGuildRanking:
                    GuildBatchService.UpdateRanking();
                    break;
                case Batch.OptimizeGuild:
                    GuildBatchService.OptimizeGuild();
                    break;
                case Batch.GiveGuildRankReward:
                    GuildBatchService.GiveGuildRankReward();
                    break;
                case Batch.PushNotification:
                    PushNotificationService.Send();
                    break;
                default:
                    throw new ArgumentException();
            }

            ContextContainer.ClearContext();
        }
    }
}
