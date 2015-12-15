using System.Collections.Generic;
using UniLinq;
using UniRx;

namespace Noroshi.BattleScene
{
    public class StoryHandler
    {
        Story _beforeBattleStory;
        Story _beforeBossWaveStory;
        Story _afterBossDieStory;
        Story _afterBattleStory;
        Dictionary<uint, ICharacterView> _characterViewMap = new Dictionary<uint, ICharacterView>();

        public StoryHandler(Core.WebApi.Response.Battle.CpuBattleStory beforeBattleStory, Core.WebApi.Response.Battle.CpuBattleStory beforeBossWaveStory, Core.WebApi.Response.Battle.CpuBattleStory afterBossDieStory,  Core.WebApi.Response.Battle.CpuBattleStory afterBattleStory)
        {
            _beforeBattleStory = beforeBattleStory != null ? new Story(beforeBattleStory) : null;
            _beforeBossWaveStory = beforeBossWaveStory != null ? new Story(beforeBossWaveStory) : null;
            _afterBossDieStory = afterBossDieStory != null ? new Story(afterBossDieStory) : null;
            _afterBattleStory = afterBattleStory != null ? new Story(afterBattleStory) : null;
        }

        public IObservable<StoryHandler> LoadDatas()
        {
            var characterIds = _getStories().SelectMany(story => story.GetCharacterIDs()).Distinct();
            var gcrm = GlobalContainer.RepositoryManager;
            return Observable.WhenAll(characterIds.Select(id => gcrm.CharacterRepository.Get(id)))
            .Do(characters => 
            {
                var characterNameTextKeyMap = characters.ToDictionary(c => c.ID, c => c.TextKey);
                if (_beforeBattleStory != null) _beforeBattleStory.SetCharacterNameTextKeyMap(characterNameTextKeyMap);
                if (_beforeBossWaveStory != null) _beforeBossWaveStory.SetCharacterNameTextKeyMap(characterNameTextKeyMap);
                if (_afterBossDieStory != null) _afterBossDieStory.SetCharacterNameTextKeyMap(characterNameTextKeyMap);
                if (_afterBattleStory != null) _afterBattleStory.SetCharacterNameTextKeyMap(characterNameTextKeyMap);
            })
            .Select(_ => this);
        }

        public IObservable<StoryHandler> LoadAssets(IFactory factory)
        {
            var characterIds = _getStories().SelectMany(story => story.GetCharacterIDs()).Distinct();
            return Observable.WhenAll(
                characterIds.Select(characterId =>
                {
                    return factory.BuildCharacterViewForUI(characterId).Do(view => _onLoadCharacterViewForUI(characterId, view));
                })
            )
            .Do(_ =>
            {
                if (_beforeBattleStory != null) _beforeBattleStory.SetCharacterViewMap(_characterViewMap);
                if (_beforeBossWaveStory != null) _beforeBossWaveStory.SetCharacterViewMap(_characterViewMap);
                if (_afterBossDieStory != null) _afterBossDieStory.SetCharacterViewMap(_characterViewMap);
                if (_afterBattleStory != null) _afterBattleStory.SetCharacterViewMap(_characterViewMap);
            })
            .Select(_ => this);
        }
        void _onLoadCharacterViewForUI(uint characterId, ICharacterView view)
        {
            _characterViewMap.Add(characterId, view);
        }
        public void Prepare()
        {
            foreach (var characterId in _characterViewMap.Keys)
            {
                _characterViewMap[characterId].SetUpForUI(CenterPositionForUIType.Story);
            }
        }

        public bool HasStory
        {
            get { return _getStories().Count() > 0; }
        }

        public Story GetBeforeBattleStory()
        {
            return _beforeBattleStory;
        }
        public Story GetBeforeBossWaveStory()
        {
            return _beforeBossWaveStory;
        }
        public Story GetAfterBossDieStory()
        {
            return _afterBossDieStory;
        }
        public Story GetAfterBattleStory()
        {
            return _afterBattleStory;
        }

        public ICharacterView GetCharacterView(uint characterId)
        {
            return _characterViewMap[characterId];
        }

        IEnumerable<Story> _getStories()
        {
            return (new Story[4]{_beforeBattleStory, _beforeBossWaveStory, _afterBossDieStory, _afterBattleStory}).Where(story => story != null);
        }
    }
}
