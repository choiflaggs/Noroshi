namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// 派遣中の傭兵キャラクターを取得する際のレスポンス。
    /// </summary>
    public class GetRentalCharactersResponse
    {
        /// <summary>
        /// エラー
        /// </summary>
        public GuildError Error { get; set; }
        /// <summary>
        /// 派遣中の傭兵キャラクター。
        /// </summary>
        public RentalCharacter[] RentalCharacters { get; set; }
    }
}
