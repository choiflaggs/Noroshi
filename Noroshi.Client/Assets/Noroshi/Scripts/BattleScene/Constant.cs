namespace Noroshi.BattleScene
{
    public class Constant
    {
        /// Wave 当たりの制限時間
        public const int WAVE_TIME = 90;
        /// ループバトル制限時間。
        public const byte LOOP_BATTLE_TIME = 60;
        /// Wave 当たり最低必要な時間。
        public const byte MIN_WAVE_BATTLE = 3;

        public const int MAX_NORMAL_CHARACTER_NUM_IN_FIELD_PER_FORCE = 5;
        public const int MAX_SHADOW_CHARACTER_NUM_IN_FIELD_PER_FORCE = 8;

        /// フィールドグリッドの横サイズ
        public const int FIELD_HORIZONTAL_GRID_SIZE = 128;
        public const int VISIBLE_FIELD_HORIZONTAL_GRID_SIZE = 64;

        // 1グリッド移動するのにかかる秒数
        public const float WALK_TIME_PER_GRID = 0.15f;

        // ゲームエンジンループの FPS
        public const int ENGINE_FPS = 60;
        // ロジックメインループの FPS
        public const int LOGIC_FPS = 10;

        // アクション間の時間目安
        public const float IDEAL_ACTION_INTERVAL = 2f;
        /// チャージアクション発動時の一時停止時間
        public const float CHARGE_ACTION_PAUSE_TIME = 1f;
        /// バトル開始前の演出時間
        public const float BATTLE_READY_TIME = 3.0f;
        public const float BATTLE_READY_ANIMATION_TIME = 2.75f;
        public const float BATTLE_READY_ZOOM_OUT_TIME = 0.5f;
        /// バトル終了時の一時停止時間
        public const float BATTLE_FINISH_PAUSE_TIME = 1.5f;
        public const float BATTLE_FINISH_SLOW_TIME = 3f;
        public const float SLOW_TIME_SCALE = 0.25f;

        public const float SWITCH_WAVE_TIME = 1f;
        public const float SLOW_SWITCH_WAVE_TIME = 4f;

        public const float DAMAGE_TIME = 0.5f;
        public const float KNOCKBACK_TIME = 0.75f;
        public const int   KNOCKBACK_DISTANCE = 3;
        public const float VANISH_TIME = 10f;

        public const float WIN_ANIMATION_WAIT_TIME = 1f;
        public const float BOSS_ESCAPE_TIME = 3f;

        public const string UI_SORTING_LAYER_NAME = "UI";
        public const int SPINE_UI_ORDER_IN_LAYER = -1;
        public const int ORDER_RANGE_IN_CHARACTER_LAYER = 1000;

        // チャージアクション時に再生するエフェクトID
        public const uint CHARGE_ACTION_CHARACTER_EFFECT_ID = 1;

        public const uint DROP_COIN_CHARACTER_EFFECT_ID = 2;

        public const uint CPU_BATTLE_BGM_SOUND_ID = 1001;
        public const uint PLAYER_BATTLE_BGM_SOUND_ID = 1002;
        public const uint CHAPTER_UI_SOUND_ID = 2001;
        public const uint FIRST_STORY_SOUND_ID = 1002;

        public const string APPEAR_ANIMATION_NAME = "appear";
        public const string IDLE_ANIMATION_NAME = "idle";
        public const string WALK_ANIMATION_NAME = "walk";
        public const string RUN_ANIMATION_NAME = "run";
        public const string DAMAGE_ANIMATION_NAME = "damage";
        public const string STUN_ANIMATION_NAME = "damage";
        public const string DEAD_ANIMATION_NAME = "dead";
        public const string ESCAPE_ANIMATION_NAME = "escape";
        public const string WIN_ANIMATION_NAME = "win";
        public const string ACTION_RANK_0_ANIMATION_NAME = "attack";
        public const string ACTION_RANK_1_ANIMATION_NAME = "a_skill1";
        public const string ACTION_RANK_2_ANIMATION_NAME = "p_skill1";
        public const string ACTION_RANK_3_ANIMATION_NAME = "p_skill2";
        public const string ACTION_RANK_4_ANIMATION_NAME = "p_skill3";
        public const string ACTION_RANK_5_ANIMATION_NAME = "p_skill4";

        public const string ACTION_ANIMATION_NAME_REGEXP = "_skill";
    }
}
