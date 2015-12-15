using System.Linq;

namespace Noroshi.Core.WebApi.Response.Battle
{
    public class BattleWave
    {
        public BattleCharacter BattleCharacter1 { get; set; }
        public BattleCharacter BattleCharacter2 { get; set; }
        public BattleCharacter BattleCharacter3 { get; set; }
        public BattleCharacter BattleCharacter4 { get; set; }
        public BattleCharacter BattleCharacter5 { get; set; }

        public BattleCharacter[] BattleCharacters
        {
            get
            {
                return (new BattleCharacter[]{
                    BattleCharacter1,
                    BattleCharacter2,
                    BattleCharacter3,
                    BattleCharacter4,
                    BattleCharacter5,
                })
                .Where(bc => bc != null).ToArray();
            }
        }
    }
}
