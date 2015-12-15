using UnityEngine;
using Noroshi.Core.Game.Enums;

namespace Noroshi.BattleScene.Bridge
{
    /// シーン遷移の橋渡しをする。このクラスだけバトルシーン外で利用しても良い。
    public class Transition
    {
        public static BattleType BattleType { get; private set; }
        public static uint[] PlayerCharacterIDs { get; private set; }
        public static BattleCategory BattleCategory { get; private set; }
        public static uint ID { get; private set; }
        public static uint PaymentNum { get; private set; }
        public static string AfterBattleSceneName { get; private set; }

        public static void TransitToCpuBattle(BattleCategory battleCategory, uint id, uint[] playerCharacterIds)
        {
            BattleType = BattleType.CpuBattle;
            BattleCategory = battleCategory;
            ID = id;
            PlayerCharacterIDs = playerCharacterIds;
            AfterBattleSceneName = Application.loadedLevelName;
            Application.LoadLevel("BattleScene");
        }
        public static void TransitToPlayerBattle(uint enemyPlayerId, uint[] playerCharacterIds)
        {
            BattleType = BattleType.PlayerBattle;
            BattleCategory = BattleCategory.Arena;
            ID = enemyPlayerId;
            PlayerCharacterIDs = playerCharacterIds;
            AfterBattleSceneName = Application.loadedLevelName;
            Application.LoadLevel("BattleScene");
        }

        /// <summary>
        /// 冒険バトルへ遷移する。
        /// </summary>
        /// <param name="expeditionStageId">冒険ステージID</param>
        /// <param name="playerCharacterIds">選択プレイヤーキャラクターID配列</param>
        public static void TransitToExpeditionBattle(uint expeditionStageId, uint[] playerCharacterIds)
        {
            BattleType = BattleType.PlayerBattle;
            BattleCategory = BattleCategory.Expedition;
            ID = expeditionStageId;
            PlayerCharacterIDs = playerCharacterIds;
            AfterBattleSceneName = Application.loadedLevelName;
            Application.LoadLevel("BattleScene");
        }

        /// <summary>
        /// ギルドレイドボスバトルへ繊維する。
        /// </summary>
        /// <param name="guildRaidBossId">ギルドレイドボス ID</param>
        /// <param name="playerCharacterIds">選択プレイヤーキャラクターID配列</param>
        /// <param name="bp">消費 BP</param>
        public static void TransitToGuildRaidBossBattle(uint guildRaidBossId, uint[] playerCharacterIds, byte bp)
        {
            BattleType = BattleType.CpuBattle;
            BattleCategory = BattleCategory.RaidBoss;
            ID = guildRaidBossId;
            PaymentNum = bp;
            PlayerCharacterIDs = playerCharacterIds;
            AfterBattleSceneName = Application.loadedLevelName;
            Application.LoadLevel("BattleScene");
        }
    }
}
