using NUnit.Framework;
using NSubstitute;
using Noroshi.BattleScene.Actions;
using Noroshi.BattleScene.Actions.Roles;
using Noroshi.Core.Game.Battle;

namespace Noroshi.Editor.Test.BattleScene.Action.Roles
{
    [TestFixture]
    public class DamageableTest
    {
        const TargetStateID DEFAULT_STATE_ID = TargetStateID.Knockback;
        const uint DEFAULT_ATTRIBUTE_ID = 1;
        const float DEFAULT_ATTRIBUTE_COEFFICIENT = 1.5f;

        [Test]
        public void ForceTransitTest()
        {
            var executor = Substitute.For<IActionExecutor>();
            
            var target = Substitute.For<IActionTarget>();
            target.IsTargetable.Returns(true);
            
            ActionEvent actionEvent = null;
            target.ReceiveActionEvent(Arg.Do<ActionEvent>(ae => actionEvent = ae));
            
            var damageable = new Damageable(
                null, null,
                null,
                null, null,
                null
                );
            damageable.Damage(executor, target, null, null);
            Assert.IsFalse(actionEvent.TargetStateID.HasValue);

            damageable = new Damageable(
                null, null,
                DEFAULT_STATE_ID,
                null, null,
                null
                );
            damageable.Damage(executor, target, null, null);
            Assert.AreEqual(DEFAULT_STATE_ID, actionEvent.TargetStateID.Value);
        }

        [Test]
        public void AddAttributeTest()
        {
            var executor = Substitute.For<IActionExecutor>();

            var target = Substitute.For<IActionTarget>();
            target.IsTargetable.Returns(true);

            ActionEvent actionEvent = null;
            target.ReceiveActionEvent(Arg.Do<ActionEvent>(ae => actionEvent = ae));

            var damageable = new Damageable(
                null, null,
                null,
                null, null,
                null
            );
            damageable.Damage(executor, target, null, null);
            Assert.IsFalse(actionEvent.AttributeID.HasValue);
            Assert.IsFalse(actionEvent.AttributeCoefficient.HasValue);

            damageable = new Damageable(
                null, null,
                null,
                DEFAULT_ATTRIBUTE_ID, DEFAULT_ATTRIBUTE_COEFFICIENT,
                null
            );
            damageable.Damage(executor, target, null, null);
            Assert.AreEqual(DEFAULT_ATTRIBUTE_ID, actionEvent.AttributeID.Value);
            Assert.AreEqual(DEFAULT_ATTRIBUTE_COEFFICIENT, actionEvent.AttributeCoefficient.Value);
        }
    }
}
