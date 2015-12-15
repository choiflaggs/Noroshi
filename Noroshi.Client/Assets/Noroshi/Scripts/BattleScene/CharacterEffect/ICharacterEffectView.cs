using UniRx;

namespace Noroshi.BattleScene.CharacterEffect
{
    public interface ICharacterEffectView : MonoBehaviours.IView
    {
        IObservable<bool> PlayOnce(ICharacterView characterView, string animationName, PositionInCharacterView position, int orderInCharacterLayer, bool unreversible = false);
        IObservable<bool> Play(ICharacterView characterView, string animationName, PositionInCharacterView position, int orderInCharacterLayer, bool unreversible = false);
        IObservable<bool> Play(ICharacterView icharacterView, string onceAnimationName, string loopAnimationName, PositionInCharacterView position, int orderInCharacterLayer);
        void Stop();
        void StopWithPlayOnce(string animationName);
        void SetOrder(int characterViewOrder, int orderInCharacterLayer);
    }
}