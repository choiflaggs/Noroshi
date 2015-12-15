using System;

namespace Noroshi.Core.WebApi.Response.Trial
{
    /// <summary>
    /// 試練。
    /// </summary>
    public class Trial
    {
        /// <summary>
        /// 試練 ID。
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// 多言語対応用テキストキー。
        /// </summary>
        public string TextKey { get; set; }
        /// <summary>
        /// タグ制約。
        /// </summary>
        public string TagFlags { get; set; }
        /// <summary>
        /// 開始日時。設定がなければ null。
        /// </summary>
        public uint? OpenedAt { get; set; }
        /// <summary>
        /// 終了日時。設定がなければ null。
        /// </summary>
        public uint? ClosedAt { get; set; }
        /// <summary>
        /// 開いている曜日。
        /// </summary>
        public DayOfWeek[] OpenDayOfWeeks { get; set; }
        /// <summary>
        /// 開いているかどうか。
        /// </summary>
        public bool IsOpen { get; set; }
        /// <summary>
        /// バトル回数。
        /// </summary>
        public byte BattleNum { get; set; }
        /// <summary>
        /// 再オープン日時。
        /// </summary>
        public uint? ReopenedAt { get; set; }
        /// <summary>
        /// 紐づいているステージ。
        /// </summary>
        public TrialStage[] Stages { get; set; }
    }
}
