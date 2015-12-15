using System.Collections.Generic;
using Noroshi.Core.WebApi.Response.Possession;
using Noroshi.Core.Game.Battle;

namespace Noroshi.Core.WebApi.Response.Battle
{
    // バトル開始通信レスポンス
    public class PlayerBattleStartResponse : IBattleStartResponse
    {
        public string SessionID { get; set; }
        public PlayerBattle Battle { get; set; }
        public List<List<List<PossessionObject>>> DropPossessionObjects { get; set; }
        public ushort PlayerExp { get; set; }

        public InitialCondition InitialCondition { get; set; }
        public AdditionalInformation AdditionalInformation { get; set; }
        public byte BattleAutoMode { get; set; }

        public BattleCharacter[] OwnCharacters { get; set; }
        /// ループバトルフラグ。
        public bool LoopBattle { get; set; }
    }
}
