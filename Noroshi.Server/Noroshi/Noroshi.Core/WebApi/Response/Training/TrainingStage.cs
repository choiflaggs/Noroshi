using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Training
{
    /// <summary>
    /// 修行ステージ。
    /// </summary>
    public class TrainingStage
    {
        /// <summary>
        /// 修行ステージ ID。
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// ステージレベル。
        /// </summary>
        public byte Level { get; set; }
        /// <summary>
        /// 必要なプレイヤーレベル。
        /// </summary>
        public ushort NecessaryPlayerLevel { get; set; }
        /// <summary>
        /// オープンしているかどうか。
        /// </summary>
        public bool IsOpen { get; set; }
        /// <summary>
        /// 過去スコア。
        /// </summary>
        public uint Score { get; set; }
    }
}
