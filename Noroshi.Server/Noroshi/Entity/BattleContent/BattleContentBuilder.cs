using System;
using Noroshi.Core.Game.Enums;

namespace Noroshi.Server.Entity.BattleContent
{
    public class BattleContentBuilder
    {
        public static IBattleContent BuildBattleContent(uint playerId, BattleCategory category, uint id)
        {
            switch (category)
            {
                case BattleCategory.Stage:
                case BattleCategory.BackStage:
                    return new StoryBattleContent(playerId, id);
                case BattleCategory.Arena:
                    return new ArenaBattleContent(playerId, id);
                case BattleCategory.Trials:
                    return new TrialBattleContent(playerId, id);
                case BattleCategory.Training:
                    return new TrainingBattleContent(playerId, id);
                case BattleCategory.Expedition:
                    return new ExpeditionBattleContent(playerId, id);
                case BattleCategory.RaidBoss:
                    return new RaidBossBattleContent(playerId, id);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
