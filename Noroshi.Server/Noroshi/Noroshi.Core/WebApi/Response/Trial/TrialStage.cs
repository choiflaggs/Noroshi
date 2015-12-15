using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Trial
{
    /// <summary>
    /// 試練ステージ。
    /// </summary>
    public class TrialStage
    {
        /// <summary>
        /// 試練ステージ ID。
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// ステージレベル。
        /// </summary>
        public byte Level { get; set; }
        /// <summary>
        /// ドロップし得る報酬。
        /// </summary>
        public PossessionObject[] DroppableRewards { get; set; }
        /// <summary>
        /// オープンしているかどうか。
        /// </summary>
        public bool IsOpen { get; set; }
        /// <summary>
        /// 過去ランク。
        /// </summary>
        public byte Rank { get; set; }
    }
}
