using Noroshi.Core.Game.Battle;

namespace Noroshi.Core.WebApi.Response.Expedition
{
    public class PlayerExpedition
    {
        /// <summary>
        /// 冒険中かどうか。ここが false だと冒険を開始する必要がある。
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 現冒険の冒険ID。冒険中でない場合は null が入っている。
        /// </summary>
        public uint? ExpeditionID { get; set; }
        /// <summary>
        /// 現冒険にてクリア済みのステップ。次回バトルはこの値 + 1 ステップの冒険ステージに挑戦となる。冒険中でない場合は null が入っている。
        /// </summary>
        public byte? ClearStep { get; set; }
        /// <summary>
        /// 報酬を獲得できるかどうかフラグ。ステージクリア後にオンになり、獲得後にオフになることを繰り返す。
        /// </summary>
        public bool CanReceiveReward { get; set; }
        /// <summary>
        /// 各プレイヤーキャラクターの状態。
        /// 初期状態プレイヤーキャラクター分はデータが存在しない。次回バトルはこの状態を引き継ぐことになる。
        /// </summary>
        public InitialCondition.PlayerCharacterCondition[] PlayerCharacterConditions { get; set; }
        /// <summary>
        /// 現冒険に所属する全冒険ステージ。
        /// </summary>
        public PlayerExpeditionStage[] Stages { get; set; }
        /// <summary>
        /// リセット回数。
        /// </summary>
        public byte ResetNum { get; set; }
        /// <summary>
        /// 最大リセット回数。
        /// </summary>
        public byte MaxResetNum { get; set; }
    }
}
