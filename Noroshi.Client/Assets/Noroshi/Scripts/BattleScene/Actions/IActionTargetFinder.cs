using System.Collections.Generic;
using Noroshi.Grid;

namespace Noroshi.BattleScene.Actions
{
    public interface IActionTargetFinder
    {
        /// アクション実行者自身をアクション対象者として取得。
        IActionTarget GetExecutorAsTarget(IActionExecutor executor);
        /// 全てのアクション対象者を取得。
        IEnumerable<IActionTarget> GetAllTargets();
        /// レンジ指定でアクション対象者を取得。
        IEnumerable<IActionTarget> GetTargetsWithHorizontalRange(GridPosition baseGridPosition, Direction horizontalDirection, int minRange, int maxRange);
    }
}
