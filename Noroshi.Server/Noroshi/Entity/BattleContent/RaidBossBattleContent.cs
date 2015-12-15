using System.Collections.Generic;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using Noroshi.Core.Game.RaidBoss;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.RaidBoss;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Entity.Battle;

namespace Noroshi.Server.Entity.BattleContent
{
    public class RaidBossBattleContent : AbstractBattleContent
    {
        GuildRaidBossEntity _guildRaidBoss;

        public RaidBossBattleContent(uint playerId, uint id)
        {
            _guildRaidBoss = GuildRaidBossEntity.ReadAndBuild(id);
        }

        public override uint GetBattleID()
        {
            return _guildRaidBoss.CpuBattleID;
        }

        public override bool CanBattle(PlayerStatusEntity playerStatus, uint paymentNum)
        {
            var playerGuildRaidBoss = PlayerGuildRaidBossEntity.CreateOrReadAndBuild(playerStatus.PlayerID, _guildRaidBoss.ID, _guildRaidBoss.CreatedAt);
            return _guildRaidBoss.CanBattle(playerStatus, playerGuildRaidBoss, paymentNum);
        }

        public override AdditionalInformation GetAdditionalInformation()
        {
            return new AdditionalInformation
            {
                WaveGaugeType = _guildRaidBoss.Type == RaidBossGroupType.Special ? WaveGaugeType.SpecialRaidBoss : WaveGaugeType.NormalRaidBoss,
                WaveGaugeTextKey = _guildRaidBoss.TextKey,
                WaveGaugeLevel = _guildRaidBoss.Level,
            };
        }

        public override InitialCondition GetInitialCondition(uint[] playerCharacterIds, uint paymentNum)
        {
            return _guildRaidBoss.GetBattleInitialCondition(playerCharacterIds, (byte)paymentNum);
        }

        public override void FinishBattle(VictoryOrDefeat victoryOrDefeat, byte rank, BattleResult result, PlayerStatusEntity playerStatus, IEnumerable<PlayerCharacterEntity> ownPlayerCharacters, GuildEntity guild)
        {
            _guildRaidBoss = GuildRaidBossEntity.ReadAndBuild(_guildRaidBoss.ID, ReadType.Lock);

            // ダメージ加算。
            PlayerGuildRaidBossEntity.AddDamage(playerStatus.PlayerID, _guildRaidBoss.ID, _guildRaidBoss.CreatedAt, result.Damage);
            // ログ追加。
            GuildRaidBossLogEntity.Create(_guildRaidBoss.ID, _guildRaidBoss.CreatedAt, playerStatus.PlayerID, result.Damage);

            var isDefeatingDamage = _guildRaidBoss.SetBattleData(playerStatus.PlayerID, result.CurrentWaveNo, result.CurrentWave.EnemyCharacterStates);
            // 勝利遷移、もしくはダメージ計算上撃破していれば撃破処理へ。
            if (victoryOrDefeat == VictoryOrDefeat.Win || isDefeatingDamage)
            {
                if (_guildRaidBoss.Defeat(playerStatus.PlayerID, guild))
                {
                    // 撃破したのが通常レイドボスの場合、特別ボスを出現抽選を試行。
                    if (_guildRaidBoss.Type == RaidBossGroupType.Normal) GuildRaidBossEntity.TryToDiscoverySpecial(playerStatus, guild, _guildRaidBoss.Level);
                }
            }
            _guildRaidBoss.Save();
        }
    }
}
