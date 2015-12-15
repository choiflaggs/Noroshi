using System.Collections.Generic;
using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response.Battle
{
    // バトル開始通信レスポンス
    public interface IBattleStartResponse
    {
        string SessionID { get; }
        List<List<List<PossessionObject>>> DropPossessionObjects { get; }
        ushort PlayerExp { get; }
        AdditionalInformation AdditionalInformation { get; }
        byte BattleAutoMode { get; }
        /// ループバトルフラグ。
        bool LoopBattle { get; }
    }
}
