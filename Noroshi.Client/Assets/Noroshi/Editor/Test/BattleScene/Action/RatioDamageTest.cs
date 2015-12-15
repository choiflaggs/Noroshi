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
    public class RatioDamageTest
    {
        const int DEFAULT_TARGET_SELECT_TYPE = 0;
        const int DEFAULT_TARGET_TAG_FLAGS = 0;
        const int DEFAULT_TARGET_SORT_TYPE = 0;
        const int DEFAULT_MAX_TARGET_NUM = 0;
        const int DEFAULT_HP_RATIO = 0;
        const int DEFAULT_ENERGY_RATIO = 0;
        const int DEFAULT_INTERCEPT1 = 10;
        const int DEFAULT_SLOPE1 = 3;
        const int DEFAULT_HORIZONTAL_INDEX = 16;

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
        [TestCase(1, 0, 0, 0, 20, 0, 1, 1, Result = new byte[]{1})]
        [TestCase(2, 2, 0, 0, 0, -50, 1, 1, Result = new byte[]{})]
        [TestCase(2, 0, 0, 0, 0, -50, 2, 1, Result = new byte[]{1, 2, 3})]
        [TestCase(3, 0, 0, 0, 30, 0, 1, 1, Result = new byte[]{3})]
        public byte[] GetTargetsTest(int targetSelectType, int targetTagFlags, int targetSortType, int maxTargetNum,
                                     int hpRatio, int energyRatio, int ownForceNum, int enemyForceNum)
        {
            _setUpMockActionTargetFiner();
            _currentAction = new RatioDamage(new Noroshi.Core.WebApi.Response.Character.Action()
            {
                Arg1 = targetSelectType,
                Arg2 = targetTagFlags,
                Arg3 = targetSortType,
                Arg4 = maxTargetNum,
                Arg5 = hpRatio,
                Arg6 = energyRatio,
                Intercept1 = DEFAULT_INTERCEPT1,
                Slope1 = DEFAULT_SLOPE1,
            });
            _setUpDefaultActionExecutor();

            for (var i = 0; i < ownForceNum; i++)
            {
                _currentMockTargetFinder.SetTarget(DEFAULT_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Own);
            }
            
            for (var i = 0; i < enemyForceNum; i++)
            {
                _currentMockTargetFinder.SetTarget(DEFAULT_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Enemy);
            }

            return _currentAction.GetTargets(_currentMockTargetFinder, _currentExecutor).Select(at => _currentMockTargetFinder.GetNoByActionTarge(at)).ToArray();
        }

        [Test]
        [TestCase(1, 2, 100, 100, 1, 1, 3, 3)]
        [TestCase(1, 2, 100, 100, 1, 0, 3, null)]
        [TestCase(1, 2, 100, 100, 0, 1, null, 3)]
        [TestCase(1, 2, 100, 100, 0, 0, null, null)]
        [TestCase(1, 6, 100, 100, 1, 0, 15, null)]
        [TestCase(1, 6, 100, 100, 0, -1, null, -15)]
        [TestCase(11, 16, 100, 100, 0, 1, null, 15)]
        public void ActionLevelUpTest(int beforeLevel, int afterLevel, uint maxHp, int maxEnergy,
                                     int hpRatio, int energyRatio, int? resultHpDamage, int? resultEnergyDamage)
        {
            _setUpMockActionTargetFiner();
            _currentAction = new RatioDamage(new Noroshi.Core.WebApi.Response.Character.Action()
            {
                Arg1 = DEFAULT_TARGET_SELECT_TYPE,
                Arg2 = DEFAULT_TARGET_TAG_FLAGS,
                Arg3 = DEFAULT_TARGET_SORT_TYPE,
                Arg4 = DEFAULT_MAX_TARGET_NUM,
                Arg5 = hpRatio,
                Arg6 = energyRatio,
                Intercept1 = DEFAULT_INTERCEPT1,
                Slope1 = DEFAULT_SLOPE1,
            });
            _setUpDefaultActionExecutor();


            _currentMockTargetFinder.GetExecutorAsTarget(_currentExecutor).MaxHP.Returns(maxHp);
            _currentMockTargetFinder.GetExecutorAsTarget(_currentExecutor).MaxEnergy.Returns((ushort)maxEnergy);

            ActionEvent actionEvent = null;
            var target = Substitute.For<IActionTarget>();
            target.ReceiveActionEvent(Arg.Do<ActionEvent>(ae => actionEvent = ae));
            target.MaxHP.Returns(maxHp);
            target.MaxEnergy.Returns((ushort)maxEnergy);
            _currentMockTargetFinder.SetTarget(DEFAULT_HORIZONTAL_INDEX, target, Force.Enemy);

            var targetCondidates = _currentAction.GetTargets(_currentMockTargetFinder, _currentExecutor);
            Assert.IsNotEmpty(targetCondidates);

            GlobalContainer.SetFactory<IRandomGenerator>(() => new MockRandomGenerator(new float[]{0.5f}));

            _currentAction.SetLevel(beforeLevel);
            _currentAction.PreProcess(_currentExecutor, targetCondidates);
            _currentAction.Execute(_currentMockTargetFinder, _currentExecutor, targetCondidates);
            var beforeHpDamage = actionEvent.HPDamage;
            var beforeEnergyDamage = actionEvent.EnergyDamage;

            _currentAction.SetLevel(afterLevel);
            _currentAction.PreProcess(_currentExecutor, targetCondidates);
            _currentAction.Execute(_currentMockTargetFinder, _currentExecutor, targetCondidates);
            var afterHpDamage = actionEvent.HPDamage;
            var afterEnergyDamage = actionEvent.EnergyDamage;

            Assert.AreEqual(resultHpDamage, afterHpDamage - beforeHpDamage);
            Assert.AreEqual(resultEnergyDamage, afterEnergyDamage - beforeEnergyDamage);
        }

        [Test]
        public void ExecuteButNoTargetTest()
        {
            _setUpDefault();
            _currentMockTargetFinder.SetTarget(DEFAULT_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Enemy);
            _currentMockTargetFinder.SetTarget(DEFAULT_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Enemy);

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

        void _setUpDefault()
        {
            _setUpDefaultAction();
            _setUpMockActionTargetFiner();
            _setUpDefaultActionExecutor();
        }

        void _setUpDefaultAction()
        {
            _currentAction = new RatioDamage(new Noroshi.Core.WebApi.Response.Character.Action()
            {
                Arg1 = DEFAULT_TARGET_SELECT_TYPE,
                Arg2 = DEFAULT_TARGET_TAG_FLAGS,
                Arg3 = DEFAULT_TARGET_SORT_TYPE,
                Arg4 = DEFAULT_MAX_TARGET_NUM,
                Arg5 = DEFAULT_HP_RATIO,
                Arg6 = DEFAULT_ENERGY_RATIO,
                Intercept1 = DEFAULT_INTERCEPT1,
                Slope1 = DEFAULT_SLOPE1,
            });
        }

        void _setUpMockActionTargetFiner()
        {
            _currentMockTargetFinder = new MockActionTargetFinder();
        }

        void _setUpDefaultActionExecutor()
        {
            _currentExecutor = Substitute.For<IActionExecutor>();
            _currentExecutor.GetDirection().Returns(Direction.Right);
            _currentMockTargetFinder.SetExecutor(DEFAULT_HORIZONTAL_INDEX, _currentExecutor, Force.Own);
        }
    }
}