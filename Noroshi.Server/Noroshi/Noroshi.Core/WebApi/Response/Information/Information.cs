using Noroshi.Core.Game.Information;

namespace Noroshi.Core.WebApi.Response.Information
{
    /// <summary>
    /// お知らせ。
    /// </summary>
    public class Information
    {
        /// <summary>
        /// お知らせID。
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// お知らせの種類。
        /// </summary>
        public InformationCategory Category { get; set; }
        /// <summary>
        /// バナー情報。
        /// </summary>
        public string BannerInformation { get; set; }
        /// <summary>
        /// 掲載開始日時。
        /// </summary>
        public uint OpenedAt { get; set; }
        /// <summary>
        /// 掲載終了日時。
        /// </summary>
        public uint ClosedAt { get; set; }
        /// <summary>
        /// お知らせのTitleTextKey。
        /// </summary>
        public string TitleTextKey { get; set; }
        /// <summary>
        /// お知らせのBodyTextKey。
        /// </summary>
        public string BodyTextKey { get; set; }
        /// <summary>
        /// 既読フラグ。
        /// </summary>
        public bool HasReadInformation { get; set; }
    }
}
