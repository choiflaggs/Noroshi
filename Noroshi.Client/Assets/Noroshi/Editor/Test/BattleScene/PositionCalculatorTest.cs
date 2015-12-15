using NUnit.Framework;
using UniLinq;
using NSubstitute;
using Noroshi.Game;
using Noroshi.BattleScene;

namespace Noroshi.Editor.Test.BattleScene
{
    [TestFixture]
    public class PositionCalculatorTest
    {
        [Test]
        [TestCase(1, 0, 1, Result = 13)]
        [TestCase(2, 0, 2, Result = 13)]
        [TestCase(2, 0, 3, Result = 13)]
        public int GetOwnCharacterVerticalIndexTest(byte noInSameCharacterPosition, byte characterPositionOffset, byte sameCharacterPositionNum)
        {
            var positionCalculator = new PositionCalculator();
            return positionCalculator.GetOwnCharacterVerticalIndex(noInSameCharacterPosition, characterPositionOffset, sameCharacterPositionNum);
        }

        [Test]
        [TestCase(1, 0, 1, Result = 12)]
        [TestCase(2, 0, 2, Result = 12)]
        [TestCase(2, 0, 3, Result = 12)]
        public int GetEnemyCharacterVerticalIndexTest(byte noInSameCharacterPosition, byte characterPositionOffset, byte sameCharacterPositionNum)
        {
            var positionCalculator = new PositionCalculator();
            return positionCalculator.GetEnemyCharacterVerticalIndex(noInSameCharacterPosition, characterPositionOffset, sameCharacterPositionNum);
        }

        [Test]
        [TestCase(Force.Own, Result = new uint[]{11,15,7,19,3,23,1,25})]
        [TestCase(Force.Enemy, Result = new uint[]{14,10,18,6,22,2,24,0})]
        public ushort[] GetEnemyCharacterVerticalIndexTest(Force force)
        {
            var positionCalculator = new PositionCalculator();
            return positionCalculator.GetShadowCharacterVerticalIndexes(force).ToArray();
        }
    }
}
