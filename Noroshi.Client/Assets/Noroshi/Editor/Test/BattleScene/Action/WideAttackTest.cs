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
    public class WideAttackTest
    {
        const int DEFAULT_TARGET_SELECT_TYPE = 0;
        const int DEFAULT_TARGET_TAG_FLAGS = 0;
        const int DEFAULT_TARGET_SORT_TYPE = 0;
        const int DEFAULT_MAX_TARFET_NUM = 0;
        const int DEFAULT_INTERCEPT1 = 10;
        const int DEFAULT_SLOPE1 = 3;
        const int DEFAULT_EXECUTOR_HORIZONTAL_INDEX = 16;
        const int DEFAULT_DAMAGE_COEFFICIENT = 0;
        const int DEFALUT_ATTACK_MIN_RANGE = -25;
        const int DEFAULT_AtTACK_MAX_RANGE = 25;

        IAction _currentAction = null;
        MockActionTargetFinder _currentMockTargetFinder = null;
        IActionExecutor _currentExecutor = null;

        [SetUp]
        public void Initialize()
        {
            _currentAction = null;
            _currentMockTargetFinder = null;
            _currentExecutor = null;
        }

        [Test]
        [TestCase(3, 0, 0, 1, -25, 25, 300, new int[]{25, 28}, new int[]{}, Result = new byte[]{})]
        [TestCase(3, 0, 0, 1, -25, 25, 300, new int[]{}, new int[]{45}, Result = new byte[]{2})]
        [TestCase(3, 0, 0, 1, -25, 25, 300, new int[]{}, new int[]{32, 33}, Result = new byte[]{2})]
        [TestCase(3, 0, 0, 1, -25, 25, 300, new int[]{25}, new int[]{32, 33}, Result = new byte[]{3})]
        [TestCase(3, 0, 0, 0, -16, 16, 0, new int[]{20}, new int[]{}, Result = new byte[]{})]
        [TestCase(3, 0, 0, 0, -16, 16, 0, new int[]{}, new int[]{33, 50, 90}, Result = new byte[]{2, 3, 4})]
        [TestCase(3, 0, 0, 0, -16, 16, 0, new int[]{13, 14, 15, 16}, new int[]{29, 30, 31, 32, 33}, Result = new byte[]{6, 7, 8, 9, 10})]
        [TestCase(1, 0, 0, 0, -16, 16, 0, new int[]{20}, new int[]{40}, Result = new byte[]{1})]
        [TestCase(11, 0, 0, 0, -25, 25, 300, new int[]{20}, new int[]{40, 41, 42, 42, 43}, Result = new byte[]{3})]
        [TestCase(12, 0, 0, 0, -25, 25, 300, new int[]{20}, new int[]{40, 41, 42, 42, 43}, Result = new byte[]{4})]
        [TestCase(13, 0, 0, 0, -25, 25, 0, new int[]{20}, new int[]{40, 41, 42, 42, 43}, Result = new byte[]{5})]
        [TestCase(14, 0, 0, 0, -25, 25, 0, new int[]{20}, new int[]{40, 41, 42, 42, 43}, Result = new byte[]{6})]
        [TestCase(15, 0, 0, 0, -25, 25, 0, new int[]{20}, new int[]{40, 41, 42, 42, 43}, Result = new byte[]{7})]
        public byte[] GetTargetsTest(int targetSelectType, int targetTagFlags, int targetSortType, int maxTargetNum,
                                     int attackMinRange, int attackMaxRange, int asynchronous,
                                     int[] ownHorizontalIndexes, int[] enemyHorizontalIndexes)
        {
            _setUpMockActionTargetFinder();
            _currentAction = new WideAttack(new Noroshi.Core.WebApi.Response.Character.Action
            {
                Arg1 = targetSelectType,
                Arg2 = targetTagFlags,
                Arg3 = targetSortType,
                Arg4 = maxTargetNum,
                Arg5 = attackMinRange,
                Arg6 = attackMaxRange,
                Arg7 = asynchronous,
                Intercept1 = DEFAULT_INTERCEPT1,
                Slope1 = DEFAULT_SLOPE1,
            });

            _currentExecutor = Substitute.For<IActionExecutor>();
            _currentExecutor.GetDirection().Returns(Direction.Right);

            _currentMockTargetFinder.SetExecutor(DEFAULT_EXECUTOR_HORIZONTAL_INDEX, _currentExecutor, Force.Own);

            foreach (var horizontalIndex in ownHorizontalIndexes)
            {
                _currentMockTargetFinder.SetTarget(horizontalIndex, Substitute.For<IActionTarget>(), Force.Own);
            }

            foreach (var horizontalIndex in enemyHorizontalIndexes)
            {
                _currentMockTargetFinder.SetTarget(horizontalIndex, Substitute.For<IActionTarget>(), Force.Enemy);
            }

            return _currentAction.GetTargets(_currentMockTargetFinder, _currentExecutor).Select(at => _currentMockTargetFinder.GetNoByActionTarge(at)).ToArray();
        }

        [Test]
        [TestCase(1, 2, 300, Result = 3)]
        [TestCase(1, 6, 300, Result = 15)]
        [TestCase(11, 16, 300, Result = 15)]
        [TestCase(1, 2, 0, Result = 3)]
        [TestCase(1, 6, 0, Result = 15)]
        [TestCase(11, 16, 0, Result = 15)]
        public int ActionLevelUpTest(int beforeLevel, int afterLevel, int asynchronous)
        {
            _setupDefault(asynchronous);

            ActionEvent actionEvent = null;

            var target = Substitute.For<IActionTarget>();
            target.ReceiveActionEvent(Arg.Do<ActionEvent>(ae => actionEvent = ae));
            _currentMockTargetFinder.SetTarget(DEFAULT_EXECUTOR_HORIZONTAL_INDEX, target, Force.Enemy);

            var targetCondidates = _currentAction.GetTargets(_currentMockTargetFinder, _currentExecutor);
            Assert.IsNotEmpty(targetCondidates);

            GlobalContainer.SetFactory<IRandomGenerator>(() => new MockRandomGenerator(new float[]{0.5f}));

            _currentAction.SetLevel(beforeLevel);
            _currentAction.PreProcess(_currentExecutor, targetCondidates);
            _currentAction.Execute(_currentMockTargetFinder, _currentExecutor, targetCondidates);
            
            var beforeDamage = actionEvent.HPDamage.Value;

            _currentAction.SetLevel(afterLevel);
            _currentAction.PreProcess(_currentExecutor, targetCondidates);
            _currentAction.Execute(_currentMockTargetFinder, _currentExecutor, targetCondidates);
            var afterDamage = actionEvent.HPDamage.Value;

            return afterDamage - beforeDamage;
        }

        [Test]
        [TestCase(300)]
        [TestCase(0)]
        public void ExecuteButNoTargetTest(int asynchronous)
        {
            _setupDefault(asynchronous);

            _currentMockTargetFinder.SetTarget(DEFAULT_EXECUTOR_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Enemy);
            _currentMockTargetFinder.SetTarget(DEFAULT_EXECUTOR_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Enemy);

            var targetCondidates = _currentAction.GetTargets(_currentMockTargetFinder, _currentExecutor);
            Assert.IsNotEmpty(targetCondidates);

            var receiveActionEventNum = 0;
            foreach (var target in _currentMockTargetFinder.GetAllTargets())
            {
                target.When(t => t.ReceiveActionEvent(Arg.Any<ActionEvent>())).Do(_ => receiveActionEventNum++);
            }
            _currentMockTargetFinder.ClearTargets();

            _currentAction.PreProcess(_currentExecutor, targetCondidates);
            _currentAction.Execute(_currentMockTargetFinder, _currentExecutor, targetCondidates);
            Assert.AreEqual(0, receiveActionEventNum);
        }
        
        void _setupDefault(int asynchronous)
        {
            _setUpDefaultActioln(asynchronous);
            _setUpMockActionTargetFinder();
            _setUpDefaultActionExecutor();
            _setUpDefaultAssets();
        }

        void _setUpDefaultActioln(int asynchronous)
        {
            _currentAction = new WideAttack(new Noroshi.Core.WebApi.Response.Character.Action
            {
                Arg1 = DEFAULT_TARGET_SELECT_TYPE,
                Arg2 = DEFAULT_TARGET_TAG_FLAGS,
                Arg3 = DEFAULT_TARGET_SORT_TYPE,
                Arg4 = DEFAULT_MAX_TARFET_NUM,
                Arg5 = DEFAULT_DAMAGE_COEFFICIENT,
                Arg6 = DEFALUT_ATTACK_MIN_RANGE,
                Arg7 = DEFAULT_AtTACK_MAX_RANGE,
                Arg8 = asynchronous,
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

        void _setUpDefaultAssets()
        {
            var factory = Substitute.For<IActionFactory>();
            var actionView = Substitute.For<IActionView>();
            actionView.Launch(Arg.Any<IActionExecutorView>(), Arg.Any<GridPosition>(), Arg.Any<Direction>(), Arg.Any<string>(), Arg.Any<int>()).Returns(Observable.Return(actionView));
            factory.BuildActionView(Arg.Any<uint>()).Returns(Observable.Return(actionView));
            _currentAction.LoadAssets(_currentExecutor, factory).Subscribe();
        }
    }
}