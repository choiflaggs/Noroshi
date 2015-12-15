using Noroshi.Core.WebApi.Response.Character;

namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// 傭兵を派遣する際のレスポンス。
    /// </summary>
    public class AddRentalCharacterResponse
    {
        /// <summary>
        /// エラー
        /// </summary>
        public GuildError Error { get; set; }
        /// <summary>
        /// 派遣したプレイヤーキャラクター。
        /// </summary>
        public PlayerCharacter PlayerCharacter { get; set; }
    }
}
