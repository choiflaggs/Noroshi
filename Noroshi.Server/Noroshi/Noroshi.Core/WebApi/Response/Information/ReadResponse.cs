
namespace Noroshi.Core.WebApi.Response.Information
{
    /// <summary>
    /// お知らせ既読更新日時。
    /// </summary>
    public class ReadResponse
    {
        /// <summary>
        /// 既読更新日時。
        /// </summary>
        public uint ConfirmedAt { get; set; }
    }
}
