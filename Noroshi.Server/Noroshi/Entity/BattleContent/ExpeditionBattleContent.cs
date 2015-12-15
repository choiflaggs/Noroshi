using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using BattleConstant = Noroshi.Core.Game.Battle.Constant;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Expedition;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Entity.BattleContent
{
    public class ExpeditionBattleContent : AbstractBattleContent
    {
        private readonly uint _id;
        PlayerExpeditionSessionEntity _playerExpeditionSession;

        public ExpeditionBattleContent(uint playerId, uint id)
        {
            _id = id;
            _playerExpeditionSession = PlayerExpeditionSessionEntity.ReadAndBuild(playerId, ReadType.Lock);
        }
        public override uint GetBattleID()
        {
            return _playerExpeditionSession.GetNextStageEnemyPlayerID().Value;
        }
        public override InitialCondition GetInitialCondition(uint[] playerCharacterIds, uint paymentNum)
        {
            return _playerExpeditionSession.GetInitialCondition(playerCharacterIds);
        }

        public override bool CanBattle(PlayerStatusEntity playerStatus, uint paymentNum)
        {
            return _playerExpeditionSession.GetNextStageID().HasValue && _playerExpeditionSession.GetNextStageID().Value == _id;
        }

        public override void FinishBattle(VictoryOrDefeat victoryOrDefeat, byte rank, BattleResult result, PlayerStatusEntity playerStatus, IEnumerable<PlayerCharacterEntity> ownPlayerCharacters, GuildEntity guild)
        {
            var playerExpeditionSession = PlayerExpeditionSessionEntity.CreateOrRead(playerStatus.PlayerID);
            if (victoryOrDefeat == VictoryOrDefeat.Win)
            {
                // 勝利時は賞品を解放。
                playerExpeditionSession.OpenReward();
            }
            // 自動回復分を反映しつつ更新。
            var automaticRecovery = playerExpeditionSession.GetExpedition().AutomaticRecovery;
            var characterMap = CharacterEntity.ReadAndBuildMulti(ownPlayerCharacters.Select(pc => pc.CharacterID)).ToDictionary(c => c.ID);
            var characterStatusMap = ownPlayerCharacters.ToDictionary(pc => pc.ID, pc => new Core.Game.Character.CharacterStatus(pc.ToBattleResponseData(), characterMap[pc.CharacterID].ToResponseData()));
            playerExpeditionSession.SetInitialOwnPlayerCharacterConditions(result.AfterBattlePlayerCharacters.Select(abpc =>
            {
                var characterStatus = characterStatusMap[abpc.PlayerCharacterID];
                var hpRecovery = automaticRecovery ? characterStatus.MaxHP * BattleConstant.HP_RECOVERY_WAVE_INTERVAL_RATIO + characterStatus.HPRegen : 0;
                var energyRecovery = automaticRecovery ? BattleConstant.MAX_ENERGY * BattleConstant.ENERGY_RECOVERY_WAVE_INTERVAL_RATIO + characterStatus.EnergyRegen : 0;
                return new InitialCondition.PlayerCharacterCondition
                {
                    PlayerCharacterID = abpc.PlayerCharacterID,
                    HP = Math.Min((uint)(abpc.HP + hpRecovery), characterStatus.MaxHP),
                    Energy = Math.Min((ushort)(abpc.Energy + energyRecovery), BattleConstant.MAX_ENERGY),
                };
            }));
            if (!playerExpeditionSession.Save())
            {
                throw new SystemException(string.Join("\t", "Fail to Save Player Expedition Session", playerExpeditionSession.PlayerID, playerExpeditionSession.ExpeditionID.Value, playerExpeditionSession.ClearStep));
            }
        }
    }
}
