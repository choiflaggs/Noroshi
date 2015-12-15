using UniRx;
using Noroshi.BattleScene.CharacterEffect;

namespace Noroshi.BattleScene
{
    public interface ICharacterFactory
    {
        IObservable<ICharacterView> BuildCharacterView(uint characterId);
        IObservable<ICharacterEffectView> BuildCharacterEffectView(uint characterEffectPrefabId);
        IObservable<ICharacterEffectView[]> BuildCharacterEffectViewMulti(uint characterEffectPrefabId, int num);
    }
}
