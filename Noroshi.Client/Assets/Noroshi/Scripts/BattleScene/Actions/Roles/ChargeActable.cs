using System;
using System.Collections;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;

namespace Noroshi.BattleScene.Actions.Roles
{
    public class ChargeActable
    {
        int _executeNum;
        int _originalHorizontalIndex;
        protected RangeSearchable _attackRangeSearchable;
        public RelativeForce? Force { get; set; }
        public ActionAnimation Animation { get; set; }
        public int FirstActionMinRange { get; set; }
        public int FirstActionMaxRange { get; set; }
        public TargetSortType? TargetSortType { get; set; }
        public int? MaxTargetNum { get; set; }
        public DamageType DamageType { get; set; }
        public int DamageBoost { get; set; }
        public DamageMagicalAttribute? DamageMagicalAttribute { get; set; }
        public uint? AttributeID { get; set; }
        public float? AttributeCoefficient { get; set; }
        public uint? HitCharacterEffectID { get; set; }
        public TargetStateID? TargetStateID { get; set; }

        public virtual void Initialize()
        {
            _attackRangeSearchable = new RangeSearchable()
            {
                Force = Force,
                MinRange = FirstActionMinRange,
                MaxRange = FirstActionMaxRange,
                MaxTargetNum = MaxTargetNum,
                SortType = TargetSortType,
            };
        }

        public virtual void PreProcess()
        {
            _executeNum = 0;
        }

        void _execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IEnumerable<IActionTarget> targets, Action actionExecute, Action<IActionExecutor> onLastExecution)
        {
            _executeNum++;
            
            if (_executeNum == 1)
            {
                var k = executor.GetDirection() == Grid.Direction.Left ? -1 : 1;
                _originalHorizontalIndex = executor.GetGridPosition().Value.HorizontalIndex;
                Func<short> getHorizontalIndex = () => {
                    var filterTargets = _attackRangeSearchable.FilterTargets(actionTargetFinder, targets);
                    var toH = filterTargets.Count() > 0 ? (int)filterTargets.Average(t => t.GetGridPosition().Value.HorizontalIndex) : _originalHorizontalIndex;
                    return (short)((toH - _originalHorizontalIndex) * k - (FirstActionMinRange + FirstActionMaxRange) / 2);
                };
                if (getHorizontalIndex() != 0) executor.HorizontalMove(getHorizontalIndex, _forwardDuration);
            }
            else if (_executeNum == 2)
            {
                // 止まるだけ
            }
            else if (_executeNum < Animation.TriggerTimes.Length - 1)
            {
                actionExecute();
            }
            else if (_executeNum == Animation.TriggerTimes.Length -1 )
            {
                onLastExecution(executor);
            }
            else if (_executeNum == Animation.TriggerTimes.Length)
            {
                // 止まるだけ
            }
        }

        public void ExecuteAndGoBack(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IEnumerable<IActionTarget> targets, Action actionExecute)
        {
            _execute(actionTargetFinder, executor, targets, actionExecute, (e) => 
            {
                var h = _originalHorizontalIndex - e.GetGridPosition().Value.HorizontalIndex;
                if (e.GetDirection() == Grid.Direction.Left) h *= -1;
                if (h != 0) e.HorizontalMove((short)h, _backwardDuration);
            });
        }

        public void ExecuteAndGoForward(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IEnumerable<IActionTarget> targets, Action actionExecute)
        {
            _execute(actionTargetFinder, executor, targets, actionExecute, (e) => 
            {
                e.GoStraight(_backwardDuration);
            });
        }
        
        float _forwardDuration { get { return Animation.TriggerTimes[1] - Animation.TriggerTimes[0]; } }
        float _backwardDuration { get { return Animation.TriggerTimes[Animation.TriggerTimes.Length - 1] - Animation.TriggerTimes[Animation.TriggerTimes.Length - 2]; } }
    }
}