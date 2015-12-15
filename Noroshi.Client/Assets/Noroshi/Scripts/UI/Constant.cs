using System;
using UnityEngine;

namespace Noroshi.UI
{
    public class Constant
    {
        public const float SCREEN_BASE_WIDTH = 1136;
        public const float SCREEN_BASE_HEIGHT = 640;

        public static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static readonly Color TEXT_COLOR_NORMAL_DARK = new Color(0.102f, 0.016f, 0.016f);
        public static readonly Color TEXT_COLOR_NORMAL_WHITE = new Color(0.961f, 0.949f, 0.886f);
        public static readonly Color TEXT_COLOR_NEGATIVE = new Color(0.604f, 0, 0);
        public static readonly Color TEXT_COLOR_POSITIVE = new Color(0.051f, 0.431f, 0);

        public static readonly Color BAR_COLOR_NORMAL = new Color(0.38f, 0.772f, 0);
        public static readonly Color BAR_COLOR_ALERT = new Color(0.882f, 0.145f, 0);

        public const string SCENE_MAIN = "Main";
        public const string SCENE_ARENA = "Arena";
        public const string SCENE_BATTLE = "BattleScene";
        public const string SCENE_CHARACTER_LIST = "CharacterList";
        public const string SCENE_DEFENSE = "Defense";
        public const string SCENE_EXPEDITION = "Expedition";
        public const string SCENE_GACHA = "Gacha";
        public const string SCENE_GUILD = "Guild";
        public const string SCENE_ITEM = "Item";
        public const string SCENE_LOGIN = "Login";
        public const string SCENE_RAID_BOSS = "RaidBoss";
        public const string SCENE_SHOP = "Shop";
        public const string SCENE_STORY = "Story";
        public const string SCENE_TRAINING = "Training";
        public const string SCENE_TRIAL = "Trial";

        public const string ANIM_IDLE = "idle";
        public const string ANIM_ATTACK = "attack";
        public const string ANIM_WALK = "walk";
        public const string ANIM_RUN = "run";
        public const string ANIM_WIN = "win";
        public const string ANIM_A_SKILL = "a_skill1";
        public const string ANIM_P_SKILL1 = "p_skill1";
        public const string ANIM_P_SKILL2 = "p_skill2";
    }
}
