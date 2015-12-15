using Noroshi.Core.WebApi.Response.Players;

namespace Noroshi.Core.WebApi.Response.Battle
{
    // バトル終了時通信レスポンス
    public class PlayerBattleFinishResponse : IBattleFinishResponse
    {
        public PlayerBattle Battle { get; set; }
        public AddPlayerExpResult AddPlayerExpResult { get; set; }
    }
}