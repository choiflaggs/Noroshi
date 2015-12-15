
namespace Noroshi.Core.WebApi.Response.Players
{
    public class PlayerError
    {
        /// <summary>
        /// BPを回復する必要がない.
        /// </summary>
        public bool NoNeedRecoverBP { get; set; }

        /// <summary>
        /// スタミナを回復する必要がない.
        /// </summary>
        public bool NoNeedRecoverStamina { get; set; }

        /// <summary>
        /// ゴールドを増やすことができない.
        /// </summary>
        public bool NoNeedRecoverGold { get; set; }

        /// <summary>
        /// スキルポイントを回復する必要がない.
        /// </summary>
        public bool NoNeedRecoverActionLevelPoint { get; set; }

        /// <summary>
        /// アリーナのクールタイムをリセットする必要がない.
        /// </summary>
        public bool NoNeedResetCoolTime { get; set; }

        /// <summary>
        /// アリーナのプレイ回数をリセットする必要がない.
        /// </summary>
        public bool NoNeedResetPlayNum { get; set; }

    }
}