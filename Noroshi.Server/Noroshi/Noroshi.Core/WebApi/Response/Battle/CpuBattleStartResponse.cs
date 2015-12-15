using System.Collections.Generic;
using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Battle
{
    // バトル開始通信レスポンス
    public class CpuBattleStartResponse : IBattleStartResponse
    {
        public string SessionID { get; set; }
        public CpuBattle Battle { get; set; }
        public List<List<List<PossessionObject>>> DropPossessionObjects { get; set; }
        public ushort PlayerExp { get; set; }

        public AdditionalInformation AdditionalInformation { get; set; }
        public byte BattleAutoMode { get; set; }

        public BattleCharacter[] OwnCharacters { get; set; }
        /// ループバトルフラグ。
        public bool LoopBattle { get; set; }
    }
}
