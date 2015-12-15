using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Guild
{
    /// <summary>
    /// 傭兵を帰還させる際のレスポンス。
    /// </summary>
    public class RemoveRentalCharacterResponse
    {
        /// <summary>
        /// エラー。
        /// </summary>
        public GuildError Error { get; set; }
        /// <summary>
        /// 帰還したプレイヤーキャラクター。
        /// </summary>
        public PlayerCharacter[] PlayerCharacters { get; set; }
        /// <summary>
        /// 最低報酬。
        /// </summary>
        public PossessionObject[] FixedRewards { get; set; }
        /// <summary>
        /// 雇用報酬。
        /// </summary>
        public PossessionObject[] RentalRewards { get; set; }
    }
}
