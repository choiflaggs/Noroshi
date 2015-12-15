using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Entity.Battle
{
    public abstract class AbstractBattleEntity : IBattleEntity
    {
        PlayerBattleSession _playerBattleSession;

        public abstract uint Gold { get; }

        public abstract uint GetCharacterExp();

        public uint[] GetPlayerCharacterIDs()
        {
            return _playerBattleSession.PlayerCharacterIDs;
        }
        public abstract void ApplyInitialCondition(InitialCondition.CharacterCondition[][] enemyCharacterConditions);

        public abstract IEnumerable<PossessionParam> GetDroppableRewards();
        public abstract List<List<List<PossessionParam>>> LotDropRewards();

        public bool StartBattle(uint playerId, IEnumerable<PlayerCharacterEntity> playerCharacters, List<List<List<PossessionParam>>> dropPossessionParams)
        {
            _playerBattleSession = PlayerBattleSession.Create(playerId, playerCharacters);
            _playerBattleSession.SetDropPossessionParams(dropPossessionParams.SelectMany(ppss => ppss.SelectMany(pps => pps.Select(pp => pp))));
            return _playerBattleSession.Save();
        }

        public void LoadSession(uint playerId, ReadType readType = ReadType.Slave)
        {
            _playerBattleSession = PlayerBattleSession.ReadAndBuildByPlayerID(playerId, readType);
        }

        public List<PossessionParam> FinishBattle(VictoryOrDefeat victoryOrDefeat)
        {
            var rewards = new List<PossessionParam>();
            if (victoryOrDefeat != VictoryOrDefeat.Win) return rewards;

            // Gold
            rewards.Add(PossessionManager.GetGoldParam(Gold));
            // ドロップ物
            rewards.AddRange(_playerBattleSession.GetDropPossessionParams());

            _playerBattleSession.Delete();

            return rewards;
        }
    }
}
