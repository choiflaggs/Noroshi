using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Entity.Character;

namespace Noroshi.Server.Entity.Battle
{
    public class PlayerBattleWave
    {
        IEnumerable<PlayerCharacterEntity> _playerCharacters;

        public PlayerBattleWave(IEnumerable<PlayerCharacterEntity> playerCharacters)
        {
            _playerCharacters = playerCharacters;
        }

        public IEnumerable<PlayerCharacterEntity> GetCharacters()
        {
            return Enumerable.Range(1, 5).Select(no => GetCharacterByNo((byte)no));
        }
        public PlayerCharacterEntity GetCharacterByNo(byte no)
        {
            var array = _playerCharacters.ToArray();
            return _playerCharacters.Count() >= no ? array[no - 1] : null;
        }

        public Core.WebApi.Response.Battle.BattleWave ToResponseData()
        {
            var character1 = GetCharacterByNo(1);
            var character2 = GetCharacterByNo(2);
            var character3 = GetCharacterByNo(3);
            var character4 = GetCharacterByNo(4);
            var character5 = GetCharacterByNo(5);
            return new Core.WebApi.Response.Battle.BattleWave()
            {
                BattleCharacter1 = character1 != null ? character1.ToBattleResponseData() : null,
                BattleCharacter2 = character2 != null ? character2.ToBattleResponseData() : null,
                BattleCharacter3 = character3 != null ? character3.ToBattleResponseData() : null,
                BattleCharacter4 = character4 != null ? character4.ToBattleResponseData() : null,
                BattleCharacter5 = character5 != null ? character5.ToBattleResponseData() : null,
            };
        }
    }
}
