using Noroshi.Core.WebApi.Response.Players;

namespace Noroshi.Core.WebApi.Response.Battle
{
    // バトル終了時通信レスポンス
    public class CpuBattleFinishResponse : IBattleFinishResponse
    {
        public CpuBattle Battle { get; set; }

        public AddPlayerExpResult AddPlayerExpResult { get; set; }

        public class DropItem
        {
            public ushort ItemID { get; private set; }
            public byte Num { get; private set; }
        }
    }
}