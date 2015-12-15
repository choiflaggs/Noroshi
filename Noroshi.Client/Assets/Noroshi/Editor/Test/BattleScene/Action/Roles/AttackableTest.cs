using NUnit.Framework;
using NSubstitute;
using Noroshi.Random;
using Noroshi.BattleScene.Actions;
using Noroshi.BattleScene.Actions.Roles;

namespace Noroshi.Editor.Test.BattleScene.Action.Roles
{
    [TestFixture]
    public class AttackableTest
    {
        [SetUp]
        public void Initialize()
        {
            GlobalContainer.Clear();
        }

        [Test]
        [TestCase(10, 5, 10, 10, null, Result = 5)]
        [TestCase(100, 5, 10, 10, null, Result = 95)]
        [TestCase(100, 5, 10, 10, 0.1f, Result = 9)]
        public int CalculatePhysicalDamageTest(uint physicalAttack, uint armorPenetration, uint physicalCrit, uint armor, float? damageCoefficient)
        {
            return CalculateDamageTest(
                DamageType.Physical,
                physicalAttack, armorPenetration, physicalCrit, armor,
                0, 0, 0, 0,
                damageCoefficient
            );
        }

        [Test]
        [TestCase(10, 5, 10, 10, null, Result = 5)]
        [TestCase(100, 5, 10, 10, null, Result = 95)]
        [TestCase(100, 10, 10, 10, 0.5f, Result = 50)]
        public int CalculateMagicalDamageTest(uint magicPower, uint ignoreMagicResistance, uint magicCrit, uint magicRegistance, float? damageCoefficient)
        {
            return CalculateDamageTest(
                DamageType.Magical,
                0, 0, 0, 0,
                magicPower, ignoreMagicResistance, magicCrit, magicRegistance,
                damageCoefficient
            );
        }

        [Test]
        [TestCase(  0, 255, 0.99f, Result = false)]
        [TestCase(  0, 255, 0.5f,  Result = false)]
        [TestCase(  0, 255, 0.0f,  Result = false)]
        [TestCase(127, 255, 0.5f,  Result = false)]
        [TestCase(127, 127, 0.5f,  Result = false)]
        [TestCase(255, 127, 0.5f,  Result = true)]
        [TestCase(255,   0, 0.0f,  Result = true)]
        [TestCase(255,   0, 0.5f,  Result = true)]
        [TestCase(255,   0, 0.99f, Result = true)]
        [TestCase( 20,  10, 0.0f,  Result = true)]
        [TestCase( 20,  10, 0.09f, Result = true)]
        [TestCase( 20,  10, 0.1f,  Result = false)]
        [TestCase( 20,  10, 0.11f, Result = false)]
        [TestCase( 20,  10, 0.99f, Result = false)]
        public bool PhysicalDodgeTest(byte dodge, byte accuracy, float percentage)
        {
            return DodgeTest(DamageType.Physical, dodge, accuracy, percentage);
        }

        [Test]
        [TestCase(  0, 255, 0.0f,  Result = false)]
        [TestCase(255,   0, 0.99f, Result = false)]
        public bool MagicalDodgeTest(byte dodge, byte accuracy, float percenttage)
        {
            return DodgeTest(DamageType.Magical, dodge, accuracy, percenttage);
        }

        public bool DodgeTest(DamageType damageType, byte dodge, byte accuracy, float percentage)
        {   
            GlobalContainer.SetFactory<IRandomGenerator>(() => new MockRandomGenerator(new float[]{percentage}));
            ActionEvent actionEvent = null;
            
            var executor = Substitute.For<IActionExecutor>();
            executor.Accuracy.Returns(accuracy);
            
            var target = Substitute.For<IActionTarget>();
            target.IsTargetable.Returns(true);
            target.Dodge.Returns(dodge);
            
            target.ReceiveActionEvent(Arg.Do<ActionEvent>(ae => actionEvent = ae));
            
            var attackable = new Attackable(damageType, null,
                                            null, null, null, null, 0, null);
            
            attackable.Attack(executor, target);
            
            return !actionEvent.HPDamage.HasValue;
        }
        
        public int CalculateDamageTest(
            DamageType damageType,
            uint physicalAttack, uint armorPenetration, uint physicalCrit, uint armor,
            uint magicPower, uint ignoreMagicResistance, uint magicCrit, uint magicRegistance,
            float? damageCoefficient
        )
        {
            GlobalContainer.SetFactory<IRandomGenerator>(() => new MockRandomGenerator(new float[]{0.5f}));
            ActionEvent actionEvent = null;

            var executor = Substitute.For<IActionExecutor>();
            executor.PhysicalAttack.Returns(physicalAttack);
            executor.ArmorPenetration.Returns(armorPenetration);
            executor.PhysicalCrit.Returns(physicalCrit);
            executor.MagicPower.Returns(magicPower);
            executor.IgnoreMagicResistance.Returns(ignoreMagicResistance);
            executor.MagicCrit.Returns(magicCrit);

            var target = Substitute.For<IActionTarget>();
            target.IsTargetable.Returns(true);
            target.Armor.Returns(armor);
            target.MagicRegistance.Returns(magicRegistance);

            target.ReceiveActionEvent(Arg.Do<ActionEvent>(ae => actionEvent = ae));

            var attackable = new Attackable(
                damageType, null,
                null,
                null, null,
                null,
                0,
                damageCoefficient
            );
            attackable.Attack(executor, target);

            return actionEvent.HPDamage.Value;
        }
    }
}
