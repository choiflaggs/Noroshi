using System;
using System.Linq;
using System.Collections.Generic;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Trial;

namespace Noroshi.Server.Entity.BattleContent
{
    public class TrialBattleContent : AbstractBattleContent
    {
        TrialEntity _trial;
        TrialStageEntity _trialStage;
        PlayerTrialEntity _playerTrial;
        PlayerTrialStageEntity _playerTrialStage;

        public TrialBattleContent(uint playerId, uint id)
        {
            _trialStage = TrialStageEntity.ReadAndBuild(id);
            if (_trialStage == null)
            {
                throw new InvalidOperationException(string.Join("\t", "Trial Stage Not Found", id));
            }
            _trial = TrialEntity.ReadAndBuild(_trialStage.TrialID);
            if (_trial == null)
            {
                throw new SystemException(string.Join("\t", "Trial Not Found", _trialStage.TrialID));
            }
            _playerTrial = PlayerTrialEntity.CreateOrReadAndBuild(playerId, _trialStage.TrialID);
            _playerTrialStage = PlayerTrialStageEntity.CreateOrReadAndBuild(playerId, _trialStage.ID);
        }
        public override uint GetBattleID()
        {
            return _trialStage.CpuBattleID;
        }

        public override bool IsValidCharacters(IEnumerable<CharacterEntity> characters)
        {
            if (!base.IsValidCharacters(characters)) return false;
            if (_trial.CharacterTagSet == null) return true;
            return characters.All(character => _trial.CharacterTagSet.GetTags().Any(tag => character.TagSet.HasTag(tag)));
        }
        public override bool CanBattle(PlayerStatusEntity playerStatus, uint paymentNum)
        {
            return _playerTrial.CanBattle(_trial, _trialStage);
        }

        public override void FinishBattle(VictoryOrDefeat victoryOrDefeat, byte rank, BattleResult result, PlayerStatusEntity playerStatus, IEnumerable<PlayerCharacterEntity> ownPlayerCharacters, GuildEntity guild)
        {
            // バトル回数をインクリメント。
            _playerTrial.IncrementBattleNum();
            // 勝利時にはクリアレベル更新試行。
            if (victoryOrDefeat == VictoryOrDefeat.Win) _playerTrial.TryToSetClearLevel(_trialStage.Level);
            if (!_playerTrial.Save())
            {
                throw new SystemException(string.Join("\t", "Fail to Save Player Trial", playerStatus.PlayerID, _playerTrial.TrialID));
            }
            // 勝利時には最高ランク更新試行。
            if (victoryOrDefeat == VictoryOrDefeat.Win && _playerTrialStage.TryToSetHightRank(rank))
            {
                if (!_playerTrialStage.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save Player Trial Stage", playerStatus.PlayerID, _playerTrialStage.TrialStageID));
                }
            }
        }
    }
}
