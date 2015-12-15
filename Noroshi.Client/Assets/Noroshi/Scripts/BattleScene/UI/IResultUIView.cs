using System.Collections.Generic;
using UniRx;

namespace Noroshi.BattleScene.UI
{
    public interface IResultUIView : IModalUIView
    {
        void SetBattleRank(byte rank);
        void SetPlayerExp(ushort exp);
        void SetMoney(uint money);
        void SetItemIDs(IEnumerable<uint> itemIds);
        void SetOwnCharacterThumbnails(IEnumerable<CharacterThumbnail> characterThumbnail);
        void SetEnemyCharacterThumbnails(IEnumerable<CharacterThumbnail> characterThumbnail);
        void SetOwnCharacterProgress(byte characterNo, float previousExpRatio, float currentExpRatio, ushort levelUpNum);
        void SetOwnCharacterStatistics(byte characterNo, uint damage, float damageRatio);
        void SetEnemyCharacterStatistics(byte characterNo, uint damage, float damageRatio);
        void SetLossTipsMessage(string tipsMessage);
        IObservable<IResultUIView> LoadAssets();
    }
}