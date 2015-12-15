using UniLinq;
using UniRx;
using Noroshi.Core.WebApi.Response.Battle;
using Noroshi.Repositories;

namespace Noroshi.Game
{
    public class Battle
    {
        CpuBattle _data;
        public Battle(CpuBattle masterData)
        {
            _data = masterData;
        }

        /// 敵情報を取得。キャラクターマスターを引くので非同期 Observable インターフェース。
        public IObservable<CharacterStatus[]> LoadCharacterStatus(IMasterDataRepository<Core.WebApi.Response.Character.Character> repository)
        {
            return repository.GetMulti(_getEnemyCharacters().Select(c => c.CharacterID).ToArray())
            .Select(masters => {
                var map = masters.ToDictionary(m => m.ID);
                return _getEnemyCharacters().Select(c => new CharacterStatus(c, map[c.CharacterID])).ToArray();
            });
        }
        public BattleCharacter[] _getEnemyCharacters()
        {
            return _data.Waves.SelectMany(w => w.BattleCharacters).ToArray();
        }
    }
}