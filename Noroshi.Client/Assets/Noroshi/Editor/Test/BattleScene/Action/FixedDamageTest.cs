using System;
using System.Collections.Generic;
using UniLinq;
using UniRx;
using NUnit.Framework;
using NSubstitute;
using Noroshi.Random;
using Noroshi.Grid;
using Noroshi.BattleScene;
using Noroshi.BattleScene.Actions;
using Noroshi.BattleScene.Actions.Roles;
using Noroshi.Editor.Test.BattleScene.Action.Mock;

namespace Noroshi.Editor.Test.BattleScene.Action
{
    [TestFixture]
    public class FixedDamageTest
    {
        const int DEFAULT_TARGET_SELECT_TYPE = 0;
        const int DEFAULT_TARGET_TAG_FLAGS = 0;
        const int DEFAULT_TARGET_SORT_TYPE = 0;
        const int DEFAULT_MAX_TARGET_NUM = 0;
        const int DEFAULT_INTERCEPT1 = 10;
        const int DEFAULT_SLOPE1 = 3;
        const int DEFAULT_EXECUTOR_HORIZONTAL_INDEX = 16;

        IAction _currentAction;
        MockActionTargetFinder _currentMockTargetFinder;
        IActionExecutor _currentExecutor;

        [SetUp]
        public void Initialize()
        {
            _currentAction = null;
            _currentMockTargetFinder = null;
            _currentExecutor = null;
        }

        /// <summary>
        /// 対象者取得のテスト。
        /// </summary>
        [Test]
        [TestCase(0, 0, 0, 0, 1, 1, Result = new byte[]{1,2,3})]
        public byte[] GetTargetsTest(int targetSelectType, int targetTagFlags, int targetrSortType, int maxTargetNum,
                                     int ownForceNum, int enemyForceNum)
        {
            _setUpMockActionTargetFinder();
            _currentAction = new FixedDamage(new Noroshi.Core.WebApi.Response.Character.Action
            {
                Arg1 = targetSelectType,
                Arg2 = targetTagFlags,
                Arg3 = targetrSortType,
                Arg4 = maxTargetNum,
                Intercept1 = DEFAULT_INTERCEPT1,
                Slope1 = DEFAULT_SLOPE1,
            });

            _currentExecutor = Substitute.For<IActionExecutor>();
            _currentExecutor.GetDirection().Returns(Direction.Right);

            _currentMockTargetFinder.SetExecutor(DEFAULT_EXECUTOR_HORIZONTAL_INDEX, _currentExecutor, Force.Own);

            for (var i = 0; i < ownForceNum; i++)
            {
                _currentMockTargetFinder.SetTarget(DEFAULT_EXECUTOR_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Own);
            }

            for (var i = 0; i < enemyForceNum; i++)
            {
                _currentMockTargetFinder.SetTarget(DEFAULT_EXECUTOR_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Enemy);
            }

            return _currentAction.GetTargets(_currentMockTargetFinder, _currentExecutor)
                .Select(at => _currentMockTargetFinder.GetNoByActionTarge(at)).ToArray();
        }
        
        /// <summary>
        /// Action Level アップ効果テスト。
        /// </summary>
        [Test]
        [TestCase(1, 2, Result = 3)]
        [TestCase(1, 6, Result = 15)]
        [TestCase(11, 16, Result = 15)]
        public int ActionLevelUpTest(int beforeLevel, int afterLevel)
        {
            _setUpDefault();

            ActionEvent actionEvent = null;
            var target = Substitute.For<IActionTarget>();
            target.ReceiveActionEvent(Arg.Do<ActionEvent>(ae => actionEvent = ae));
            _currentMockTargetFinder.SetTarget(DEFAULT_EXECUTOR_HORIZONTAL_INDEX, target, Force.Enemy);

            var targetCandidates = _currentAction.GetTargets(_currentMockTargetFinder, _currentExecutor);
            Assert.IsNotEmpty(targetCandidates);

            GlobalContainer.SetFactory<IRandomGenerator>(() => (new MockRandomGenerator(new float[]{0.5f})));

            _currentAction.SetLevel(beforeLevel);
            _currentAction.PreProcess(_currentExecutor, targetCandidates);
            _currentAction.Execute(_currentMockTargetFinder, _currentExecutor, targetCandidates);
            var beforeHpHpDamage = actionEvent.HPDamage.Value;

            _currentAction.SetLevel(afterLevel);
            _currentAction.PreProcess(_currentExecutor, targetCandidates);
            _currentAction.Execute(_currentMockTargetFinder, _currentExecutor, targetCandidates);
            var afterHpDamage = actionEvent.HPDamage.Value;

            return afterHpDamage - beforeHpHpDamage;
        }
        
        /// <summary>
        /// Action を実行するが、実行するまでの間に対象者がいなくなってしまった場合のテスト。
        /// </summary>
        [Test]
        public void ExecuteButNoTargetTest()
        {
            _setUpDefault();

            _currentMockTargetFinder.SetTarget(DEFAULT_EXECUTOR_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Enemy);
            _currentMockTargetFinder.SetTarget(DEFAULT_EXECUTOR_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Enemy);
            
            var targetCandidates = _currentAction.GetTargets(_currentMockTargetFinder, _currentExecutor);
            Assert.IsNotEmpty(targetCandidates);

            var reciveActionEventBum = 0;
            foreach (var target in _currentMockTargetFinder.GetAllTargets())
            {
                target.When(t => t.ReceiveActionEvent(Arg.Any<ActionEvent>())).Do(_ => reciveActionEventBum++);
            }
            _currentMockTargetFinder.ClearTargets();
            _currentAction.PreProcess(_currentExecutor, targetCandidates);
            _currentAction.Execute(_currentMockTargetFinder, _currentExecutor, targetCandidates);
            Assert.AreEqual(0, reciveActionEventBum);
        }

        void _setUpDefault()
        {
            _setUpDefaultAction();
            _setUpMockActionTargetFinder();
            _setUpDefaultActionExecutor();
        }

        void _setUpDefaultAction()
        {
            _currentAction = new FixedDamage(new Noroshi.Core.WebApi.Response.Character.Action
            {
                Arg1 = DEFAULT_TARGET_SELECT_TYPE,
                Arg2 = DEFAULT_TARGET_TAG_FLAGS,
                Arg3 = DEFAULT_TARGET_SORT_TYPE,
                Arg4 = DEFAULT_MAX_TARGET_NUM,
                Intercept1 = DEFAULT_INTERCEPT1,
                Slope1 = DEFAULT_SLOPE1,
            });
        }

        void _setUpMockActionTargetFinder()
        {
            _currentMockTargetFinder = new MockActionTargetFinder();
        }

        void _setUpDefaultActionExecutor()
        {
            _currentExecutor = Substitute.For<IActionExecutor>();
            _currentExecutor.GetDirection().Returns(Direction.Right);
            _currentMockTargetFinder.SetExecutor(DEFAULT_EXECUTOR_HORIZONTAL_INDEX, _currentExecutor, Force.Own);
        }
    }
}