using System;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public interface IMonoBehaviour : IDisposable
    {
        bool SetActive(bool active);
        void SetParent(IMonoBehaviour parent, bool worldPositionStays);
        void RemoveParent();
    }
}
