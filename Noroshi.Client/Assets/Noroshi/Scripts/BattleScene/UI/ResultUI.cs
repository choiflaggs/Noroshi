using System.Collections.Generic;
using UniLinq;
using UniRx;

namespace Noroshi.BattleScene.UI
{
    public class ResultUI : AbstractModalUIViewModel<IResultUIView>
    {
        readonly string _name;
        public ResultUI(string name)
        {
            _name = name;
        }
        protected override IObservable<IResultUIView> _loadView()
        {
            return SceneContainer.GetFactory().BuildResultUIView(_name);
        }
        public IObservable<IResultUIView> LoadView(
            byte rank, ushort playerExp, uint money, IEnumerable<uint> dropItemIds,
            IEnumerable<CharacterThumbnail> ownCharacterThumbnails, IEnumerable<CharacterThumbnail> enemyCharacterThumbnails,
            uint[] ownDamages, uint[] enemyDamages
        )
        {
            return LoadView().SelectMany(view =>
            {
                view.SetBattleRank(rank);
                view.SetPlayerExp(playerExp);
                view.SetMoney(money);
                view.SetItemIDs(dropItemIds);
                view.SetOwnCharacterThumbnails(ownCharacterThumbnails);
                view.SetEnemyCharacterThumbnails(enemyCharacterThumbnails);
                var totalDamage = (uint)(ownDamages.Sum(d => d) + enemyDamages.Sum(d => d));
                for (byte no = 1; no <= ownDamages.Length; no++)
                {
                    view.SetOwnCharacterStatistics(no, ownDamages[no - 1], (float)ownDamages[no - 1] / totalDamage);
                }
                for (byte no = 1; no <= enemyDamages.Length; no++)
                {
                    view.SetEnemyCharacterStatistics(no, enemyDamages[no - 1], (float)enemyDamages[no - 1] / totalDamage);
                }
                return view.LoadAssets();
            })
            .Select(_ => _uiView);
        }
    }
}