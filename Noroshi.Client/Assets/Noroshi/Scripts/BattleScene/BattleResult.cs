using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.Core.Game.Enums;

namespace Noroshi.BattleScene
{
    public class BattleResult
    {
        Core.WebApi.Response.Battle.IBattleFinishResponse _finishResponse;
        VictoryOrDefeat _victoryOrDefeat;
        Subject<VictoryOrDefeat> _onEnterWinCharacterAnimationSubject = new Subject<VictoryOrDefeat>();

        public BattleResult(VictoryOrDefeat victoryOrDefeat)
        {
            _victoryOrDefeat = victoryOrDefeat;
        }

        public VictoryOrDefeat GetVictoryOrDefeat()
        {
            return _victoryOrDefeat;
        }

        public Core.WebApi.Response.Players.AddPlayerExpResult GetAddPlayerExpResult()
        {
            return _finishResponse.AddPlayerExpResult;
        }
        public void SetFinishResponse(Core.WebApi.Response.Battle.IBattleFinishResponse finishResponse)
        {
            _finishResponse = finishResponse;
        }

        /// バトル結果のランク
        public byte GetRank()
        {
            var deadCharacterNum = SceneContainer.GetCharacterManager().GetCurrentOwnCharacters().Where(c => c.IsDead).Count();
            return (byte)(deadCharacterNum == 0 ? 3 : deadCharacterNum == 1 ? 2 : 1);
        }

        public IObservable<VictoryOrDefeat> GetOnEnterWinCharacterAnimation()
        {
            return _onEnterWinCharacterAnimationSubject.AsObservable();
        }

        public IObservable<VictoryOrDefeat> PlayCharacterWinAnimation()
        {
            var aliveCharacters = SceneContainer.GetBattleManager().CurrentWave.Field.GetAllCharacters().Where(c => c.GetType() != typeof(ShadowCharacter)).ToArray();
            for (var i = 0; i < aliveCharacters.Length; i++)
            {
                aliveCharacters[i].TryToIdleAtFinishBattle();
            }
            return SceneContainer.GetTimeHandler().Timer(Constant.WIN_ANIMATION_WAIT_TIME)
                .Do(_ => {
                    _onEnterWinCharacterAnimationSubject.OnNext(_victoryOrDefeat);
                    _onEnterWinCharacterAnimationSubject.OnCompleted();
                })
                .SelectMany(_ => _victoryOrDefeat != VictoryOrDefeat.Draw ? aliveCharacters.Select(character => character.Win()).Merge().Buffer(aliveCharacters.Length) : Observable.Return<IList<bool>>(null))
                .Select(_ => _victoryOrDefeat);
        }

        public void Dispose()
        {
        }
    }
}
