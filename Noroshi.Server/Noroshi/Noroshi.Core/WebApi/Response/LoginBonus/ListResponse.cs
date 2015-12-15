namespace Noroshi.Core.WebApi.Response.LoginBonus
{
    /// <summary>
    /// 指定プレイヤーのログインボーナス一覧を取得した際のレスポンス。
    /// </summary>
    public class ListResponse
    {
        /// <summary>
        /// ログインボーナス一覧。
        /// </summary>
        public LoginBonus[] LoginBonuses { get; set; }
    }
}
