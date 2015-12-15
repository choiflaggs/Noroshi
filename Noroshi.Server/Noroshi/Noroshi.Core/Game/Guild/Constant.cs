using System;
using System.Collections.Generic;

namespace Noroshi.Core.Game.Guild
{
    public class Constant
    {
        /// <summary>
        /// 通常ギルドに所属できり最大プレイヤー数。
        /// </summary>
        public const byte MAX_NORMAL_GUILD_MEMBER_NUM = 50;
        /// <summary>
        /// ギルド設立費用。
        /// </summary>
        public const ushort NECESSARY_GEM_TO_CREATE_NORMAL_GUILD = 300;
        /// <summary>
        /// ギルド名最小文字数。
        /// </summary>
        public const byte MIN_NAME_LENGTH = 1;
        /// <summary>
        /// ギルド名最大文字数。
        /// </summary>
        public const byte MAX_NAME_LENGTH = 32;
        /// <summary>
        /// 紹介文最小文字数。
        /// </summary>
        public const byte MIN_INTRODUCTION_LENGTH = 1;
        /// <summary>
        /// 紹介文最大文字数。
        /// </summary>
        public const byte MAX_INTRODUCTION_LENGTH = 160;
        /// <summary>
        /// デフォルトギルド名フォーマット。
        /// </summary>
        public const string DEFAULT_GUILD_NAME_FORMAT_TEXT_KEY = "Guild.Default.Name";
        /// <summary>
        /// デフォルト紹介文フォーマット。
        /// </summary>
        public const string DEFAULT_GUILD_INTRODUCTION_FORMAT_TEXT_KEY = "Guild.Default.Introduction";

        /// <summary>
        /// ギルド最適化（強制除名、強制解散）対象ギルドとなるのに必要な時間。
        /// </summary>
        public static readonly TimeSpan NECESSARY_TIME_TO_OPTIMIZE = TimeSpan.FromDays(7 + 1);
        /// <summary>
        /// 強制ギルド解散されないために必要なギルドBP消費値。
        /// </summary>
        public const uint BREAK_GUILD_WEEKLY_BP_CONSUMING_THRESHOLD = 1;
        /// <summary>
        /// 強制ギルド除名されないために必要な最低プレイヤースタミナ消費値。
        /// </summary>
        public const uint LAYOFF_PLAYER_WEEKLY_STAMINA_CONSUMING_THRESHOLD = 1;
        /// <summary>
        /// ギルドランクに必要な友情ポイント。
        /// </summary>
        public static readonly Dictionary<GuildRank, ushort> GUILD_RANK_TO_NECESSARY_COOPERATION_POINT_MAP = new Dictionary<GuildRank, ushort>
        {
            { GuildRank.S, 501 },
            { GuildRank.A, 201 },
            { GuildRank.B, 101 },
            { GuildRank.C,  51 },
            { GuildRank.D,  11 },
            { GuildRank.E,   0 },
        };

        /// <summary>
        /// 初心者ギルド内での最大日次挨拶数。
        /// </summary>
        public const byte MAX_GREETING_NUM_PER_DAY_IN_BEGINNER_GUILD = 5;
        /// <summary>
        /// 挨拶毎に獲得できる BP。
        /// </summary>
        public const byte BP_PER_GREETING = 1;
        /// <summary>
        /// 被挨拶毎に獲得できるギルドポイント。
        /// </summary>
        public const byte GUILD_POINT_PER_GREETED = 10;

        /// <summary>
        /// 傭兵期間。
        /// </summary>
        public static readonly TimeSpan RENTAL_SPAN = TimeSpan.FromHours(12);
        /// <summary>
        /// 最小傭兵番号。
        /// </summary>
        public const byte MIN_PLAYER_RENTAL_CHARACTER_NO = 1;
        /// <summary>
        /// 最大傭兵雇用報酬受け取り可能数。
        /// </summary>
        public const byte MAX_RECEIVABLE_RENTAL_REWARD_NUM = 5;
        /// <summary>
        /// キャラクターレベル当たりの傭兵最低報酬ゴールド。
        /// </summary>
        public const ushort RENTAL_FIXED_REWARD_GOLD_PER_CHARACTER_LEVEL = 1200;
        /// <summary>
        /// 傭兵最低報酬ジェム。
        /// </summary>
        public const byte RENTAL_FIXED_REWARD_GEM = 60;
        /// <summary>
        /// キャラクターレベル当たりの傭兵雇用報酬ゴールド。
        /// </summary>
        public const ushort RENTAL_REWARD_GOLD_PER_CHARACTER_LEVEL = 1000;
        /// <summary>
        /// 傭兵雇用報酬ジェム。
        /// </summary>
        public const byte RENTAL_REWARD_GEM= 20;
        /// <summary>
        /// 初心者ギルド脱退後もチャットに読み書きできる期限(unixTimeSpan)。
        /// </summary>
        public static readonly TimeSpan BEGINNER_GUILD_CHAT_DROPOUT_AFTER_SPAN = TimeSpan.FromDays(30);
        /// <summary>
        /// 初心者ギルド脱退後もチャットに読み書きできる期限
        /// </summary>
        public const  ushort   GUILD_CHAT_ROW_LIMIT = 30;
    }
}
