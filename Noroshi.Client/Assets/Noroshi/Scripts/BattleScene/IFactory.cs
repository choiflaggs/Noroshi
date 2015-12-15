using UniRx;

namespace Noroshi.BattleScene
{
    public interface IFactory : ICharacterFactory, Actions.IActionFactory
    {
        IObservable<IFieldView> BuildFieldView(uint fieldId);

        IObservable<UI.IUIController> BuildUIController();
        IObservable<UI.IOwnCharacterPanelUIView> BuildOwnCharacterPanelUIView(uint characterId, int skinLevel);
        IObservable<UI.ICharacterAboveUIView> BuildCharacterAboveUIView();
        IObservable<UI.ICharacterAboveUIView[]> BuildCharacterAboveUIViewMulti(int num);
        IObservable<UI.ICharacterTextUIView[]> BuildCharacterTextUIMulti(int num);
        IObservable<UI.ICharacterTextUIView> BuildCharacterTextUI();

        IObservable<UI.IModalUIView> BuildModalUIView(string name);
        IObservable<UI.IResultUIView> BuildResultUIView(string name);
        IObservable<UI.IStoryUIView> BuildStoryUIView();

        IObservable<UI.IDropItemUIView> BuildDropItemUIView(uint itemId);
        IObservable<UI.IDropMoneyUIView> BuildDropMoneyUIView();

        IObservable<ICharacterView> BuildCharacterViewForUI(uint characterId);

        IObservable<Camera.ICameraController> BuildCameraController();
    }
}
