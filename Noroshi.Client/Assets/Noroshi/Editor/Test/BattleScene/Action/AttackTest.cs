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
    public class AttackTest
    {
        const int DEFAULT_TARGET_SELECT_TYPE = 0;
        const int DEFAULT_TARGET_TAG_INDEX = 0;
        const int DEFAULT_TARGET_SORT_TYPE = 0;
        const int DEFAULT_MAX_TARGET_NUM = 0;
        const int DEFAULT_DAMAGE_COEFFICIENT = 0;
        const int DEFAULT_INTERCEPT1 = 10;
        const int DEFAULT_SLOPE1 = 3;
        const int DEFAULT_HORIZONTAL_INDEX = 16;

        IAction _currentAction;
        MockActionTargetFinder _currentMockTargetFinder;
        IActionExecutor _currentExecutor;
        IActionView _currentActionView;

        [SetUp]
        public void Initialize()
        {
            _currentAction = null;
            _currentMockTargetFinder = null;
            _currentExecutor = null;
            _currentActionView = null;
        }

        /// <summary>
        /// 対象者取得のテスト。
        /// </summary>
        [Test]
        [TestCase(0, 0, 0, 0, 1, 1, Result = new byte[]{1,2,3})]
        public byte[] GetTargetsTest(
            int targetSelectType, int targetTagFlags, int targetSortType, int maxTargetNum,
            int ownForceNum, int enemyForceNum
        )
        {
            _setUpMockActionTargetFinder();

            _currentAction = new Attack(new Core.WebApi.Response.Character.Action
            {
                Arg1 = targetSelectType,
                Arg2 = targetTagFlags,
                Arg3 = targetSortType,
                Arg4 = maxTargetNum,
                Intercept1 = DEFAULT_INTERCEPT1,
                Slope1 = DEFAULT_SLOPE1,
            });

            _currentExecutor = Substitute.For<IActionExecutor>();
            _currentExecutor.GetDirection().Returns(Direction.Right);
            
            _currentMockTargetFinder.SetExecutor(DEFAULT_HORIZONTAL_INDEX, _currentExecutor, Force.Own);
            
            for (var i = 0; i < ownForceNum; i++)
            {
                _currentMockTargetFinder.SetTarget(DEFAULT_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Own);
            }
            
            for (var i = 0; i < enemyForceNum; i++)
            {
                _currentMockTargetFinder.SetTarget(DEFAULT_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Enemy);
            }
            
            // 何番目に追加した対象者なのかを配列で取得（上記の通り、実行者、味方、敵の順に追加処理）。
            return _currentAction.GetTargets(_currentMockTargetFinder, _currentExecutor).Select(at => _currentMockTargetFinder.GetNoByActionTarge(at)).ToArray();
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

            // 対象者をセット。
            ActionEvent actionEvent = null;
            var target = Substitute.For<IActionTarget>();
            target.ReceiveActionEvent(Arg.Do<ActionEvent>(ae => actionEvent = ae));
            _currentMockTargetFinder.SetTarget(DEFAULT_HORIZONTAL_INDEX, target, Force.Enemy);
            
            // 対象者検索。一応、存在することはチェックしておく。
            var targetCandidates = _currentAction.GetTargets(_currentMockTargetFinder, _currentExecutor);
            Assert.IsNotEmpty(targetCandidates);
            
            // 乱数によるダメージのブレは消しておく。
            GlobalContainer.SetFactory<IRandomGenerator>(() => new MockRandomGenerator(new float[]{0.5f}));
            
            // レベルアップ前HPダメージ取得。
            _currentAction.SetLevel(beforeLevel);
            _currentAction.PreProcess(_currentExecutor, targetCandidates);
            _currentAction.Execute(_currentMockTargetFinder, _currentExecutor, targetCandidates);
            var beforeHpDamage = actionEvent.HPDamage.Value;
            
            // レベルアップ後HPダメージ取得。
            _currentAction.SetLevel(afterLevel);
            _currentAction.PreProcess(_currentExecutor, targetCandidates);
            _currentAction.Execute(_currentMockTargetFinder, _currentExecutor, targetCandidates);
            var afterHpDamage = actionEvent.HPDamage.Value;
            
            // ダメージがいくつ増えたか。
            return afterHpDamage - beforeHpDamage;
        }
        
        /// <summary>
        /// Action を実行するが、実行するまでの間に対象者がいなくなってしまった場合のテスト。
        /// </summary>
        [Test]
        public void ExecuteButNoTargetTest()
        {
            _setUpDefault();

            // 適当に対象者をセット。
            _currentMockTargetFinder.SetTarget(DEFAULT_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Enemy);
            _currentMockTargetFinder.SetTarget(DEFAULT_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Enemy);
            
            // 初回判定時（実際にはアニメーション開始時）には対象が存在していることを確認。
            var targetCandidates = _currentAction.GetTargets(_currentMockTargetFinder, _currentExecutor);
            Assert.IsNotEmpty(targetCandidates);

            // ActionTargetFinder 内のアクション対象者が ActionEvent を受け取った回数を数えられるように処理を挟んだ上で、
            // ActionTargetFinder からはクリア。今後の検索では対象者が引っかからないようにしてしまう。
            var receiveActionEventNum = 0;
            foreach (var target in _currentMockTargetFinder.GetAllTargets())
            {
                target.When(t => t.ReceiveActionEvent(Arg.Any<ActionEvent>())).Do(_ => receiveActionEventNum++);
            }
            _currentMockTargetFinder.ClearTargets();
            
            // Action を実行し、どの対象者も ActionEvent を受け取っていないことを確認。
            _currentAction.PreProcess(_currentExecutor, targetCandidates);
            _currentAction.Execute(_currentMockTargetFinder, _currentExecutor, targetCandidates);
            Assert.AreEqual(0, receiveActionEventNum);
        }

        [Test]
        public void CancelTest()
        {
            _setUpDefault();
            
            // 適当に対象者をセット。
            _currentMockTargetFinder.SetTarget(DEFAULT_HORIZONTAL_INDEX, Substitute.For<IActionTarget>(), Force.Enemy);
            
            // 初回判定時（実際にはアニメーション開始時）には対象が存在していることを確認。
            var targetCandidates = _currentAction.GetTargets(_currentMockTargetFinder, _currentExecutor);
            Assert.IsNotEmpty(targetCandidates);

            // Action を実行し、すぐにキャンセルを呼ぶ
            _currentAction.PreProcess(_currentExecutor, targetCandidates);
            _currentAction.Execute(_currentMockTargetFinder, _currentExecutor, targetCandidates);

            // Cancel内でちゃんと_view.Disappear()が呼ばれた回数をカウントアップする
            var counter = 0;
            _currentActionView.When(actionView => actionView.Disappear()).Do(actionView => counter++);
            _currentAction.Cancel(_currentExecutor);

            Assert.AreEqual(1, counter);
        }

        void _setUpDefault()
        {
            _setUpDefaultAction();
            _setUpMockActionTargetFinder();
            _setUpDefaultActionExecutor();
            _setUpDefaultAssets();
        }
        void _setUpDefaultAction()
        {
            _currentAction = new Attack(new Core.WebApi.Response.Character.Action
            {
                Arg1 = DEFAULT_TARGET_SELECT_TYPE,
                Arg2 = DEFAULT_TARGET_TAG_INDEX,
                Arg3 = DEFAULT_TARGET_SORT_TYPE,
                Arg4 = DEFAULT_MAX_TARGET_NUM,
                Arg5 = DEFAULT_DAMAGE_COEFFICIENT,
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
            _currentMockTargetFinder.SetExecutor(DEFAULT_HORIZONTAL_INDEX, _currentExecutor, Force.Own);
        }
        void _setUpDefaultAssets()
        {
            _currentActionView = Substitute.For<IActionView>();
            var factory = Substitute.For<IActionFactory>();
            factory.BuildActionView(Arg.Any<uint>()).Returns(Observable.Return(_currentActionView));
            _currentAction.LoadAssets(_currentExecutor, factory).Subscribe();
        }
    }
}
