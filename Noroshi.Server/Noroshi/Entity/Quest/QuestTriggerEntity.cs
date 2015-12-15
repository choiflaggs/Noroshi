using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.GameContent;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Core.Game.Character;
using Noroshi.Server.Entity.Possession;


namespace Noroshi.Server.Entity.Quest
{
    public class QuestTriggerEntity
    {
        enum TriggerID
        {
            StoryBattleNum              =  1,   // 表ストーリー.
            TrialBattleNum              =  2,   // 試練.
            ArenaBattleNum              =  3,   // アリーナ.
            ExpeditionBattleNum         =  4,   // 冒険.
            BackStoryBattleNum          =  5,   // 裏ストーリー.
            TrainingBattleNum           =  6,   // 修行.
            RaidBossBattleNum           =  7,   // レイドボス.              :TODO 開発中 後でつなぐ.

            PlayerLevelNum              =  8,   // プレイヤーレベル.
            GreetingNum                 =  9,   // 挨拶.
            GearEnchantNum              = 10,   // 武器練成.                :TODO 開発中 後でつなぐ.
            ConvertGemToGoldNum         = 11,   // ゴールド換金.            :TODO 開発中 後でつなぐ.
            BuyItemShopNum              = 12,   // ショップアイテム.

            FreeGachaNum                = 13,   // 無料ガチャ.
            TotalGachaNum               = 14,   // ガチャページのガチャ実行累計.

            CharactersNum               = 15,   // キャラ所持数.
            CharacterLevelUpNum         = 16,   // キャラのレベルアップ回数.
            CharacterActionLevelUpNum   = 17,   // スキルレベルアップ.
            CharactersRareFiveNum       = 18,   // レア5超えキャラ数.
            PromotionGreenCharacterNum  = 19,   // 枠色緑キャラ数.
            PromotionBlueCharacterNum   = 20,   // 枠色青キャラ数.
            PromotionPurpleCharacterNum = 21,   // 枠色紫キャラ数.
            PromotionOrangeCharacterNum = 22,   // 枠色橙キャラ数.

            TimedStaminaRecoverMorning  = 23,   // 時限式スタミナ回復朝.
            TimedStaminaRecoverNoon     = 24,   // 時限式スタミナ回復昼.
            TimedStaminaRecoverNight    = 25,   // 時限式スタミナ回復夕.

            TimedBPRecoveryMorning      = 26,   // 時限式BP回復朝.
            TimedBPRecoveryNoon         = 27,   // 時限式BP回復昼.
            TimedBPRecoveryNight        = 28,   // 時限式BP回復夕.
        }

        public static bool CountUpBattleFinishNum(uint playerId, BattleCategory battleCategory)
        {
            var entity = _buildBattleFinishTriggerByBattleCategory(battleCategory);
            if (entity == null) return false;

            var quests = QuestEntity.ReadAndBuildByTriggerID(entity.ID);
            var dailyQuests = DailyQuestEntity.ReadAndBuildByTriggerID(entity.ID);

            if (quests.Count() > 0)
            {
                PlayerQuestTriggerEntity.CountUp(playerId, entity.ID);
            }
            if (dailyQuests.Count() > 0)
            {
                PlayerDailyQuestTriggerEntity.CountUp(playerId, entity.ID);
            }

 			return true;
        }

        public static bool CountUpPlayerCharacterPromotionColorNum(uint playerId, byte promotionRank)
        {
            switch (promotionRank)
            {
                case 2:
         			return CountUpPromotionGreenCharacterNum(playerId);
                                    
                case 3:
         			return CountUpPromotionBlueCharacterNum(playerId);

                case 4:
         			return CountUpPromotionPurpleCharacterNum(playerId);
                                    
                case 5:
         			return CountUpPromotionOrangeCharacterNum(playerId);

                default:
         			return false;
            }
        }
        public static bool CountUpPlayerCharacterEvolutionLevelNum(uint playerId, uint evolutionLevel)
        {
            switch (evolutionLevel)
            {
                case 5:
         			return CountUpRareFiveCharactersNum(playerId);
                    
                default:
         			return false;
            }
        }

        public static bool CountUpExecutedGachaNum(uint playerId, PossessionParam possessionParam)
        {
            // クエスト：無料ガチャ実行回数.
            if (PossessionManager.IsGoldParam(possessionParam)) CountUpFreeGachaNum(playerId);

            // クエスト：ガチャページのガチャ実行累計.
            CountUpTotalGachaNum(playerId);

 			return true;
        }


        public static bool CountUpPlayerLevelNum(uint playerId)
        {
			return _countUpExecuted(new QuestTriggerEntity(TriggerID.PlayerLevelNum), playerId);
        }

        public static bool CountUpGreetingNum(uint playerId)
        {
			return _countUpExecuted(new QuestTriggerEntity(TriggerID.GreetingNum), playerId);
        }

        public static bool CountUpGearEnchantNum(uint playerId)
        {
            //TODO: 作成後に繋ぎ込み
 			return _countUpExecuted(new QuestTriggerEntity(TriggerID.GearEnchantNum), playerId);
        }

        public static bool CountUpConvertGemToGoldNum(uint playerId)
        {
			return _countUpExecuted(new QuestTriggerEntity(TriggerID.ConvertGemToGoldNum), playerId);
        }

        public static bool CountUpBuyItemShopNum(uint playerId)
        {
			return _countUpExecuted(new QuestTriggerEntity(TriggerID.BuyItemShopNum), playerId);
        }

        public static bool CountUpFreeGachaNum(uint playerId)
        {
			return _countUpExecuted(new QuestTriggerEntity(TriggerID.FreeGachaNum), playerId);
        }

        public static bool CountUpTotalGachaNum(uint playerId)
        {
			return _countUpExecuted(new QuestTriggerEntity(TriggerID.TotalGachaNum), playerId);
        }

        public static bool CountUpCharactersNum(uint playerId)
        {
			return _countUpExecuted(new QuestTriggerEntity(TriggerID.CharactersNum), playerId);
        }

        public static bool CountUpCharacterActionLevelUpNum(uint playerId, ushort add)
        {
            if (0 <= add) return false;
 			return _countUpExecuted(new QuestTriggerEntity(TriggerID.CharacterActionLevelUpNum), playerId, add);
        }

        public static bool CountUpCharacterLevelUpNum(uint playerId, uint add)
        {
			return _countUpExecuted(new QuestTriggerEntity(TriggerID.CharacterLevelUpNum), playerId, add);
        }

        public static bool CountUpRareFiveCharactersNum(uint playerId)
        {
			return _countUpExecuted(new QuestTriggerEntity(TriggerID.CharactersRareFiveNum), playerId);
        }

        public static bool CountUpPromotionGreenCharacterNum(uint playerId)
        {
			return _countUpExecuted(new QuestTriggerEntity(TriggerID.PromotionGreenCharacterNum), playerId);
        }

        public static bool CountUpPromotionBlueCharacterNum(uint playerId)
        {
			return _countUpExecuted(new QuestTriggerEntity(TriggerID.PromotionBlueCharacterNum), playerId);
        }

        public static bool CountUpPromotionPurpleCharacterNum(uint playerId)
        {
 			return _countUpExecuted(new QuestTriggerEntity(TriggerID.PromotionPurpleCharacterNum), playerId);
        }
        public static bool CountUpPromotionOrangeCharacterNum(uint playerId)
        {
 			return _countUpExecuted(new QuestTriggerEntity(TriggerID.PromotionOrangeCharacterNum), playerId);
        }

        static bool _countUpExecuted(QuestTriggerEntity entity, uint playerId, uint add = 1)
        {
            if (entity == null) return false;

            var quests = QuestEntity.ReadAndBuildByTriggerID(entity.ID);
            var dailyQuests = DailyQuestEntity.ReadAndBuildByTriggerID(entity.ID);

            if (quests.Count() == 0 && dailyQuests.Count() == 0) return false;

            if (quests.Count() > 0)
            {
                PlayerQuestTriggerEntity.CountUp(playerId, entity.ID, add);
            }
            if (dailyQuests.Count() > 0)
            {
                PlayerDailyQuestTriggerEntity.CountUp(playerId, entity.ID, add);
            }
 			return true;
        }

        static readonly Dictionary<BattleCategory, TriggerID> BATTLE_CATEGORY_TO_TRIGGER_ID_MAP = new Dictionary<BattleCategory, TriggerID>
        {
            {BattleCategory.Stage, TriggerID.StoryBattleNum},
            {BattleCategory.Trials, TriggerID.TrialBattleNum},
            {BattleCategory.Arena, TriggerID.ArenaBattleNum},
            {BattleCategory.Expedition, TriggerID.ExpeditionBattleNum},

            {BattleCategory.BackStage, TriggerID.BackStoryBattleNum},
            {BattleCategory.Training, TriggerID.TrainingBattleNum},
            {BattleCategory.RaidBoss, TriggerID.RaidBossBattleNum},
        };
        static readonly Dictionary<TriggerID, GameContentID> TRIGGER_GAME_CONTENT_MAP = new Dictionary<TriggerID, GameContentID>
        {
            {TriggerID.StoryBattleNum, Core.Game.GameContent.GameContentID.Story},
            {TriggerID.TrialBattleNum, Core.Game.GameContent.GameContentID.Trial},
            {TriggerID.ArenaBattleNum, Core.Game.GameContent.GameContentID.Arena},
            {TriggerID.ExpeditionBattleNum, Core.Game.GameContent.GameContentID.Expedition},

            {TriggerID.BackStoryBattleNum, Core.Game.GameContent.GameContentID.BackStory},
            {TriggerID.TrainingBattleNum, Core.Game.GameContent.GameContentID.Training},
            {TriggerID.RaidBossBattleNum, Core.Game.GameContent.GameContentID.Story},
        };

        static QuestTriggerEntity _buildBattleFinishTriggerByBattleCategory(BattleCategory battleCategory)
        {
            if (!BATTLE_CATEGORY_TO_TRIGGER_ID_MAP.ContainsKey(battleCategory)) return null;
 			return new QuestTriggerEntity(BATTLE_CATEGORY_TO_TRIGGER_ID_MAP[battleCategory]);
        }

        TriggerID _triggerId;

        public QuestTriggerEntity(uint triggerId)
        {
            _triggerId = (TriggerID)triggerId;
        }
        QuestTriggerEntity(TriggerID triggerId)
        {
            _triggerId = triggerId;
        }

        public uint ID => (uint)_triggerId;

        public uint? GameContentID => TRIGGER_GAME_CONTENT_MAP.ContainsKey(_triggerId) ? (uint?)TRIGGER_GAME_CONTENT_MAP[_triggerId] : null;


        // 時間帯で報酬を与えるタイプの TriggerID.
        static readonly uint[] QuestTypeTimeZoneMatche = new uint[]
        {
            (uint)TriggerID.TimedBPRecoveryMorning,
            (uint)TriggerID.TimedBPRecoveryNoon,
            (uint)TriggerID.TimedBPRecoveryNight,
            (uint)TriggerID.TimedStaminaRecoverMorning,
            (uint)TriggerID.TimedStaminaRecoverNoon,
            (uint)TriggerID.TimedStaminaRecoverNight,
        };

        /// <summary>
        /// QuestEntity.TriggerIDに格納されているuint値が時限式クエストか判断する.
        /// </summary>
        /// <param name="triggerId">対象プレイヤーID</param>
        /// <param name="dailyQuestId">対象デイリー依頼ID</param>
        /// <returns>bool</returns>
        public static bool IsQuestTypeTimeZone(uint triggerId)
        {
 			return QuestTypeTimeZoneMatche.Contains(triggerId);
        }

    }
}
