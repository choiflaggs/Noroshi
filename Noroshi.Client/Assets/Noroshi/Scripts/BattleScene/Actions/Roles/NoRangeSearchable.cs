using System;
using System.Collections.Generic;
using UniLinq;

namespace Noroshi.BattleScene.Actions.Roles
{
    public enum TargetSelectType
    {
        Self = 1,
        OwnForce = 2,
        OtherForce = 3,
        EnmeyPositionFront = 11,
        EnmeyPositionFront2nd = 12,
        EnmeyPositionCenter = 13,
        EnmeyPositionBack2nd = 14,
        EnmeyPositionBack = 15,
        OwnPositionFront = 21,
        OwnPositionFront2nd = 22,
        OwnPositionCenter = 23,
        OwnPositionBack2nd = 24,
        OwnPositionBack = 25,
    }

    public class NoRangeSearchable : AbstractSearchable
    {
        public TargetSelectType? SelectType { get; set; }
        public int? TagIndex { get; set; }
        public Trigger Trigger { get; set; }
        public override int? MaxTargetNum { get; set; }
        public override TargetSortType? SortType { get; set; }

        public IEnumerable<IActionTarget> Search(IActionTargetFinder actionTargetFinder, IActionExecutor executor)
        {
            IEnumerable<IActionTarget> targets = null;
            if (!SelectType.HasValue)
            {
                targets = actionTargetFinder.GetAllTargets();
            }
            else if (SelectType == TargetSelectType.Self)
            {
                targets = new IActionTarget[]{actionTargetFinder.GetExecutorAsTarget(executor)};
            }
            else if (SelectType == TargetSelectType.OwnForce)
            {
                targets = actionTargetFinder.GetAllTargets().Where(t => t.Force == executor.CurrentForce);
            }
            else if (SelectType == TargetSelectType.OtherForce)
            {
                targets = actionTargetFinder.GetAllTargets().Where(t => t.Force != executor.CurrentForce);
            }
            else if (_isEnmeyPositionSelectType(SelectType.Value))
            {
                targets = _getEnmeyForceActiveTargetByPosition(actionTargetFinder, executor, SelectType.Value);
            }
            else if (_isOwnPositionSelectType(SelectType.Value))
            {
                targets = _getrOwnForceActiveTargetByPosition(actionTargetFinder, executor, SelectType.Value);
            }
            else
            {
                throw new Exception("Invalid master.");
            }
            // タグフィルタリング
            if (targets != null && TagIndex.HasValue)
            {
                targets = targets.Where(t => t.TagSet.HasTag(TagIndex.Value)).ToArray();
            }
            // ソート
            if (SortType.HasValue)
            {
                targets = _sort(targets, executor.CurrentForce);
            }
            return FilterTargets(actionTargetFinder, targets);
        }

        bool _isOwnPositionSelectType(TargetSelectType selectType)
        {
            return (
                selectType == TargetSelectType.OwnPositionFront ||
                selectType == TargetSelectType.OwnPositionFront2nd ||
                selectType == TargetSelectType.OwnPositionCenter ||
                selectType == TargetSelectType.OwnPositionBack2nd ||
                selectType == TargetSelectType.OwnPositionBack
                );
        }
        bool _isEnmeyPositionSelectType(TargetSelectType selectType)
        {
            return (
                selectType == TargetSelectType.EnmeyPositionFront ||
                selectType == TargetSelectType.EnmeyPositionFront2nd ||
                selectType == TargetSelectType.EnmeyPositionCenter ||
                selectType == TargetSelectType.EnmeyPositionBack2nd ||
                selectType == TargetSelectType.EnmeyPositionBack
            );
        }

        protected IEnumerable<IActionTarget> _getrOwnForceActiveTargetByPosition(IActionTargetFinder actionTargetFinder, IActionExecutor executor, TargetSelectType selectType)
        {
            var characters = executor.CurrentForce == Force.Own
                ? actionTargetFinder.GetAllTargets().Where(t => t.Force == executor.CurrentForce).OrderByDescending(t => t.GetGridPosition().Value.HorizontalIndex).ToArray()
                : actionTargetFinder.GetAllTargets().Where(t => t.Force == executor.CurrentForce).OrderBy(t => t.GetGridPosition().Value.HorizontalIndex).ToArray();
            if (characters.Length == 0) return new IActionTarget[]{};
            var index
                = selectType == TargetSelectType.OwnPositionFront    ? 0 
                : selectType == TargetSelectType.OwnPositionFront2nd ? (int)(characters.Length / 4)
                : selectType == TargetSelectType.OwnPositionCenter   ? (int)(characters.Length / 2)
                : selectType == TargetSelectType.OwnPositionBack2nd  ? (int)(characters.Length / 4f * 3)
                : selectType == TargetSelectType.OwnPositionBack     ? characters.Length - 1 : 0;
            return new IActionTarget[1]{characters[index]};
        }
        protected IEnumerable<IActionTarget> _getEnmeyForceActiveTargetByPosition(IActionTargetFinder actionTargetFinder, IActionExecutor executor, TargetSelectType selectType)
        {
            var characters = executor.CurrentForce == Force.Own
                ? actionTargetFinder.GetAllTargets().Where(t => t.Force != executor.CurrentForce).OrderBy(t => t.GetGridPosition().Value.HorizontalIndex).ToArray()
                : actionTargetFinder.GetAllTargets().Where(t => t.Force != executor.CurrentForce).OrderByDescending(t => t.GetGridPosition().Value.HorizontalIndex).ToArray();
            if (characters.Length == 0) return new IActionTarget[]{};
            var index
                = selectType == TargetSelectType.EnmeyPositionFront    ? 0 
                : selectType == TargetSelectType.EnmeyPositionFront2nd ? (int)(characters.Length / 4)
                : selectType == TargetSelectType.EnmeyPositionCenter   ? (int)(characters.Length / 2)
                : selectType == TargetSelectType.EnmeyPositionBack2nd  ? (int)(characters.Length / 4f * 3)
                : selectType == TargetSelectType.EnmeyPositionBack     ? characters.Length - 1 : 0;
            return new IActionTarget[1]{characters[index]};
        }

        public override IEnumerable<IActionTarget> FilterTargets(IActionTargetFinder actionTargetFinder, IEnumerable<IActionTarget> targets)
        {
            //  
            if (Trigger == Trigger.Dead)
            {
                if (SelectType.HasValue && SelectType.Value == TargetSelectType.Self)
                {
                    return targets;
                }
            }
            return base.FilterTargets(actionTargetFinder, targets);
        }
    }
}
