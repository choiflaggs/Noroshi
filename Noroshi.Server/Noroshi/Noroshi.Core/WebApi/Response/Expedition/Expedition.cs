namespace Noroshi.Core.WebApi.Response.Expedition
{
    public class Expedition
    {
        /// <summary>
        /// 冒険ID。
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// レベル。
        /// </summary>
        public byte Level { get; set; }
        /// <summary>
        /// 自動回復有無。
        /// </summary>
        public bool AutomaticRecovery { get; set; }
    }
}
