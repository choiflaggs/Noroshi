using UniRx;

namespace Noroshi.BattleScene.UI
{
    public interface IUIController
    {
        void DeactiveLoadingUIView();

        IObservable<bool> ActivateHeaderAndFooter();
        IObservable<bool> DeactivateHeaderAndFooter();

        void AddModalUIView(IModalUIView uiView);
        void AddResultUIView(IResultUIView uiView);
        void AddPlayerCharacterPanelUI(IOwnCharacterPanelUIView uiView);

        void SetCurrentMoneyNum(uint num);
        void SetCurrentItemNum(byte num);
        void SetCurrentWaveNum(int num);
        void SetMaxWaveNum(int num);

        void SetNextWaveButtonVisible(bool visible);
        IObservable<bool> GetOnClickNextWaveButtonObservable();

        IObservable<bool> GetOnClickPauseButtonObservable();

        IObservable<bool> GetOnToggleAutoModeObservable();

        void UpdateTime(int time);

        void SetToWorldUICanvas(MonoBehaviours.IUIView uiView);
        MonoBehaviours.IUIView GetTextUICanvas();

        IObservable<float> PlayPrepareAnimation();
        void PlayWinMessage();

        IObservable<IUIController> ActivateChapterUIView(string titleTextKey);

        IObservable<IUIController> ActivateResultCharacterMessageUIView(ICharacterView characterView, string message, bool win);

        IObservable<bool> DarkenWorld();
        IObservable<bool> LightenWorld();

        void InitializeWaveGaugeView(byte? level, string textKey, uint nowHP, uint maxHP, Noroshi.Core.Game.Battle.WaveGaugeType waveGaugeType);
        void ChangeWaveGauge(float ratio);
    }
}
