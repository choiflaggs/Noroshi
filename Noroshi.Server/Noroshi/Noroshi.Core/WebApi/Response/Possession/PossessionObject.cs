namespace Noroshi.Core.WebApi.Response.Possession
{
    /// <summary>
    /// クライアントに返される、保有できる汎用的な「モノ」を表現するクラス。
    /// </summary>
    public class PossessionObject
    {
        /// <summary>
        /// カテゴリ。「モノ」が何であるかを表現する。Noroshi.Core.Game.Enums.PossessionCategory に変換して利用。
        /// </summary>
        public byte Category { get; set; }
        /// <summary>
        /// 該当カテゴリ内でのID。カテゴリがアイテムの場合はアイテムIDとなる。
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// 数量。
        /// </summary>
        public uint Num { get; set; }
        /// <summary>
        /// 名前。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 保有数。
        /// </summary>
        public uint PossessingNum { get; set; }
    }
}
