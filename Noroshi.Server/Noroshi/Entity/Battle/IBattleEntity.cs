using System.Collections.Generic;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Entity.Battle
{
    public interface IBattleEntity
    {
        uint[] GetPlayerCharacterIDs();
        uint GetCharacterExp();
        void ApplyInitialCondition(InitialCondition.CharacterCondition[][] enemyCharacterConditions);
        IEnumerable<PossessionParam> GetDroppableRewards();
        List<List<List<PossessionParam>>> LotDropRewards();
        void LoadSession(uint playerId, ReadType readType = ReadType.Slave);
        bool StartBattle(uint playerId, IEnumerable<PlayerCharacterEntity> playerCharacters, List<List<List<PossessionParam>>> dropPossessionParams);
        List<PossessionParam> FinishBattle(VictoryOrDefeat victoryOrDefeat);
    }
}
