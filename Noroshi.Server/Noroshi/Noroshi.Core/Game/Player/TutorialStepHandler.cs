using System.Collections.Generic;

namespace Noroshi.Core.Game.Player
{
    /// <summary>
    /// チュートリアル状態を扱うクラス。
    /// </summary>
    public class TutorialStepHandler
    {
        static readonly Dictionary<uint, TutorialStep> STAGE_ID_TO_STEP_MAP = new Dictionary<uint, TutorialStep>
        {
            { 10102, TutorialStep.ClearStoryStage1 },
            { 10103, TutorialStep.ClearStoryStage2 },
            { 10203, TutorialStep.ClearStoryStage3 },
            { 10204, TutorialStep.ClearStoryStage4 },
            { 10205, TutorialStep.ClearStoryStage5 },
            { 10301, TutorialStep.ClearStoryStage6 },
            { 10305, TutorialStep.ClearStoryStage7 },
            { 10405, TutorialStep.ClearStoryStage8 },
            { 10510, TutorialStep.ClearStoryStage9 },
            { 10619, TutorialStep.ClearStoryStage10 },
        };

        public ushort Step { get; private set; }

        public TutorialStepHandler(ushort step)
        {
            Step = step;
        }

        public bool ProceedByClearStageID(uint clearStageId)
        {
            if (!STAGE_ID_TO_STEP_MAP.ContainsKey(clearStageId)) return false;
            var nextStep = STAGE_ID_TO_STEP_MAP[clearStageId];
            return Proceed(nextStep);
        }

        public bool Proceed(TutorialStep nextStep)
        {
            if (Step < (ushort)nextStep)
            {
                Step = (ushort)nextStep;
                return true;
            }
            return false;
        }

        public bool CanLotTutorialGacha { get { return Step < (ushort)TutorialStep.LotGacha; } }
    }
}
