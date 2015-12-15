namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// バトルに連れていくことが可能な傭兵キャラクターを取得する際のレスポンス。
    /// </summary>
    public class GetTakableRentalCharactersResponse
    {
        /// <summary>
        /// 利用可能な傭兵プレイヤーキャラクター。
        /// </summary>
        public PlayerCharacter[] RentalPlayerCharacters { get; set; }
    }
}
