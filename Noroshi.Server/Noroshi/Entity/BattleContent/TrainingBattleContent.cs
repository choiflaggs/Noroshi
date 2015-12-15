using System;
using System.Collections.Generic;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using Noroshi.Core.Game.Training;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Training;
using Noroshi.Server.Entity.Possession;

namespace Noroshi.Server.Entity.BattleContent
{
    public class TrainingBattleContent : AbstractBattleContent
    {
        TrainingEntity _training;
        TrainingStageEntity _trainingStage;
        PlayerTrainingEntity _playerTraining;
        PlayerTrainingStageEntity _playerTrainingStage;
        ushort _defeatingNum;
        uint _damage;

        public TrainingBattleContent(uint playerId, uint id)
        {
            _trainingStage = TrainingStageEntity.ReadAndBuild(id);
            if (_trainingStage == null)
            {
                throw new InvalidOperationException(string.Join("\t", "Training Stage Not Found", id));
            }
            _training = TrainingEntity.ReadAndBuild(_trainingStage.TrainingID);
            if (_training == null)
            {
                throw new SystemException(string.Join("\t", "Training Not Found", _trainingStage.TrainingID));
            }
            _playerTraining = PlayerTrainingEntity.CreateOrReadAndBuild(playerId, _trainingStage.TrainingID);
            _playerTrainingStage = PlayerTrainingStageEntity.CreateOrReadAndBuild(playerId, _trainingStage.ID);
        }
        public override uint GetBattleID()
        {
            return _trainingStage.CpuBattleID;
        }

        public override bool IsLoopBattle()
        {
            return _training.Type == TrainingType.DefeatingNum;
        }

        public override bool CanBattle(PlayerStatusEntity playerStatus, uint paymentNum)
        {
            return _playerTraining.CanBattle(_training, _trainingStage, playerStatus.Level);
        }

        public override void FinishBattle(VictoryOrDefeat victoryOrDefeat, byte rank, BattleResult result, PlayerStatusEntity playerStatus, IEnumerable<PlayerCharacterEntity> ownPlayerCharacters, GuildEntity guild)
        {
            // バトル回数をインクリメント。
            _playerTraining.IncrementBattleNum();
            if (!_playerTraining.Save())
            {
                throw new SystemException(string.Join("\t", "Fail to Save Player Training", playerStatus.PlayerID, _playerTraining.TrainingID));
            }
            // スコアの更新を試みる。
            uint? score
                = _training.Type == TrainingType.DefeatingNum ? result.DefeatingNum
                : _training.Type == TrainingType.Damage ? (uint?)result.Damage
                : null;
            if (!score.HasValue) throw new SystemException();

            if (_playerTrainingStage.TryToSetHightScore(score.Value))
            {
                if (!_playerTrainingStage.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save Player Training Stage", playerStatus.PlayerID, _playerTrainingStage.TrainingStageID));
                }
            }
            // TODO : 後で整理。
            _defeatingNum = result.DefeatingNum;
            _damage = result.Damage;
        }

        public override IEnumerable<PossessionParam> GetRewards(PlayerStatusEntity playerStatus, VictoryOrDefeat victoryOrDefeat)
        {
            return _trainingStage.GetRewards(_training, _defeatingNum, _damage);
        }
    }
}
