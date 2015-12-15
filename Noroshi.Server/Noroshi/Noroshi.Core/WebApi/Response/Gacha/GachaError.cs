namespace Noroshi.Core.WebApi.Response.Gacha
{
    public class GachaError
    {
        /// <summary>
        /// エントリーポイントが見つからない。
        /// </summary>
        public bool EntryPointNotFound { get; set; }
        /// <summary>
        /// 抽選ができない。
        /// </summary>
        public bool CannotLot { get; set; }
    }
}
