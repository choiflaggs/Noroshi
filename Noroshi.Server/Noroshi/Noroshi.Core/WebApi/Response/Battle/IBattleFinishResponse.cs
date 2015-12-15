using Noroshi.Core.WebApi.Response.Players;

namespace Noroshi.Core.WebApi.Response.Battle
{
    // バトル終了時通信レスポンス
    public interface IBattleFinishResponse
    {
        AddPlayerExpResult AddPlayerExpResult { get; set; }
    }
}