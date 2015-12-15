using System;
namespace Noroshi.Core.Game.Arena
{
    public class Constant
    {
        /// <summary>
        /// リスティングで表示するプレイヤー数.
        /// </summary>
        public const byte ARENA_LISTING_PLAYER_NUM                  = 3;
        /// <summary>
        /// リスティングで継ぎ足し検索を行う上限.
        /// </summary>
        public const byte ARENA_LISTING_SEARCH_LIMIT_NUM            = 3;
        /// <summary>
        /// リスティングでDBから取得する人数.
        /// </summary>
        public const byte ARENA_RANKING_SEARCH_RANGE_NUM            = 100;
        /// <summary>
        /// リスティングでDBから取得する際の継ぎ足し人数.
        /// </summary>
        public const byte ARENA_RANKING_SEARCH_EXTEND_RANGE_NUM     = 50;
        /// <summary>
        /// アリーナのディリーの対戦回数の上限.
        /// </summary>
        public const byte ARENA_DAILY_PLAY_LIMIT_NUM                = 5;
        /// <summary>
        /// 戦闘後のクールタイム.
        /// </summary>
        public static readonly TimeSpan ARENA_COOLTIME_AFTRE_BATTLE_SPAN = TimeSpan.FromMinutes(10);
        /// <summary>
        /// リスティングから除外する時間　戦闘から経過時間による制限.
        /// </summary>
        public static readonly TimeSpan ARENA_LISTING_NEXT_BATTLE_SPAN   = TimeSpan.FromSeconds(120);
        /// <summary>
        /// アリーナ　初回に配布するレーティングポイント.
        /// </summary>
        public const uint ARENA_START_RATING_POINTS = 1500;
        /// <summary>
        /// アリーナ　課金クールタイムリセット.
        /// </summary>
        public const byte GEM_RESET_COOLTIME_NUM = 40;
        /// <summary>
        /// アリーナ　課金デイリー制限リセット.
        /// </summary>
        public const byte GEM_RESET_PLAY_NUM = 100;
    }
}