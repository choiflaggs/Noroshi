namespace Noroshi.BattleScene.UI
{
    public interface IPlayerLevelUpModalView : UI.IModalUIView
    {
        void SetAddPlayerExpResult(Core.WebApi.Response.Players.AddPlayerExpResult addPlayerExpResult);
    }
}
