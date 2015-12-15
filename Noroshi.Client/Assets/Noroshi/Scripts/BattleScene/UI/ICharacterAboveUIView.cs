namespace Noroshi.BattleScene.UI
{
    public interface ICharacterAboveUIView : MonoBehaviours.IUIView
    {
        void SetTarget(ICharacterView characterView, Force force, uint initialHp, uint maxHp);
        void ChangeHPRatio(float ratio);
        void ChangeShieldRatio(float ratio);
        void Reset();
    }
}
