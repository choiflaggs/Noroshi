namespace Noroshi.Core.Game.Information
{
    /// <summary>
    /// お知らせの種別。
    /// PlayerConfirmationのReferenceIDの時は2からなため、各自＋1の値
    /// </summary>
    public enum InformationCategory
    {
        /// <summary>
        /// イベント開始。
        /// </summary>
        Event = 1,
        /// <summary>
        /// キャンペーン開始。
        /// </summary>
        Campaign = 2,
        /// <summary>
        /// 重要なお知らせ。
        /// </summary>
        Important = 3,
        /// <summary>
        /// お詫び。
        /// </summary>
        Apology = 4,
    }

}
