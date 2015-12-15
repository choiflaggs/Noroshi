using UnityEngine;
using UniLinq;
using UniRx;
using Noroshi.BattleScene;
using Noroshi.BattleScene.UI;

namespace NoroshiDebug.BattleResultUI
{
    public class Main : MonoBehaviour
    {
        [SerializeField] Factory _factory;

        [SerializeField] bool _win = true;

        [SerializeField] byte _rank = 1;
        [SerializeField] ushort _playerExp = 1;
        [SerializeField] ushort _money = 10;
        [SerializeField] uint[] _itemIds;
        [SerializeField] SerializableCharacterThumbnail[] _ownCharacterThumbnails;
        [SerializeField] SerializableCharacterThumbnail[] _enemyCharacterThumbnails;

        [SerializeField] float[] _ownPreviousExpRatio;
        [SerializeField] float[] _ownCurrentExpRatio;
        [SerializeField] ushort[] _ownLevelUpNum;

        [SerializeField] uint[] _ownStats;
        [SerializeField] uint[] _enemyStats;

        void Start()
        {
            _factory.LoadResultUIView(_win ? "WinUI" : "LossUI")
            .SelectMany(v => _factory.LoadUIController().Do(c => c.AddResultUIView(v)).Select(_ => v))
            .Do(_setParams)
            .SelectMany(v => v.Open())
            .Subscribe();
        }

        void _setParams(IResultUIView view)
        {
            if (_win)
            {
                view.SetBattleRank(_rank);
                view.SetPlayerExp(_playerExp);
                view.SetMoney(_money);
                view.SetItemIDs(_itemIds);
            }
            view.SetOwnCharacterThumbnails(_ownCharacterThumbnails.Select(ch => new CharacterThumbnail(ch.CharacterID, ch.Level, ch.EvolutionLevel, ch.PromotionLevel, ch.SkinLevel, ch.IsDead)));
            view.SetEnemyCharacterThumbnails(_enemyCharacterThumbnails.Select(ch => new CharacterThumbnail(ch.CharacterID, ch.Level, ch.EvolutionLevel, ch.PromotionLevel, ch.SkinLevel, ch.IsDead)));
            for (byte no = 1; no <= _ownStats.Length; no++)
            {
                view.SetOwnCharacterProgress(no, _ownPreviousExpRatio[no - 1], _ownCurrentExpRatio[no - 1], _ownLevelUpNum[no - 1]);
            }
            var totalDamage = (uint)(_ownStats.Sum(d => d) + _enemyStats.Sum(d => d));
            for (byte no = 1; no <= _ownStats.Length; no++)
            {
                view.SetOwnCharacterStatistics(no, _ownStats[no - 1], (float)_ownStats[no - 1] / totalDamage);
            }
            for (byte no = 1; no <= _enemyStats.Length; no++)
            {
                view.SetEnemyCharacterStatistics(no, _enemyStats[no - 1], (float)_enemyStats[no - 1] / totalDamage);
            }
        }
    }
}