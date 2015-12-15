using UniRx;

namespace Noroshi.BattleScene.UI
{
    public interface IOwnCharacterPanelUIView : MonoBehaviours.IUIView
    {
        IObservable<bool> GetOnClickObservable();
        void ChangeHP(ChangeableValueEvent hpEvent);
        void ChangeEnergy(ChangeableValueEvent energyEvent);
        void ToggleActiveActionAvailable(bool available);
        void Initialize(int activeActionLevel, uint initializeHp, uint maxHp, int initializeEnergy, int maxEnergy);
        void ChangeStatusBoost(CharacterStatusBoostEvent boostEvent);
        void FinishActiveAction();
        void EnterActiveAction();
    }
}
