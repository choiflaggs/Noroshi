using System;
using UniRx;

namespace Noroshi.BattleScene.UI
{
    public interface ICharacterTextUIView : IDisposable
    {
        bool SetActive(bool active);
        IObservable<UI.ICharacterTextUIView> Appear(MonoBehaviours.IUIView parent, UnityEngine.Vector2 aboveUIPosition, string text);
        IObservable<UI.ICharacterTextUIView> AppearHPDifference(MonoBehaviours.IUIView parent, UnityEngine.Vector2 aboveUIPosition, int difference);
        IObservable<UI.ICharacterTextUIView> AppearEnergyDifference(MonoBehaviours.IUIView parent, UnityEngine.Vector2 aboveUIPosition, int difference);
    }
}
