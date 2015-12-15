using UniRx;
using Noroshi.Core.Game.Enums;
using Noroshi.BattleScene.Sound;

namespace Noroshi.BattleScene.UI
{
    public class CpuBattleUIManager : AbstractUIManager
    {
        StoryUI _storyUI = new StoryUI();
        Subject<bool> _onExitBeforeBattleStorySubject = new Subject<bool>();
        public override void Initialize()
        {
            base.Initialize();
            var battleManager = (CpuBattleManager)SceneContainer.GetBattleManager();
            SceneContainer.GetSceneManager().GetOnEnterReadyObservable()
            .Select(_ => battleManager.GetBeforeBattleStory())
            .Where(story => story != null)
            .SelectMany(story =>
            {
                if (!string.IsNullOrEmpty(battleManager.GetTitleTextKey()))
                {
                    _onCommandSoundSubject.OnNext(new SoundEvent()
                    {
                        Command = SoundCommand.Play,
                        SoundID = Constant.CHAPTER_UI_SOUND_ID,
                    });
                    return _uiController.ActivateChapterUIView(battleManager.GetTitleTextKey()).Select(_ => story);
                }
                else
                {
                    return Observable.Return<Story>(story);
                }
            })
            .SelectMany(story =>
            {
                _onCommandSoundSubject.OnNext(new SoundEvent()
                {
                    Command = SoundCommand.Play,
                    SoundID = Constant.FIRST_STORY_SOUND_ID,
                });
                return _storyUI.Start(story);
            })
            .Subscribe(_ => _exitBeforeBattleStory()).AddTo(_disposables);

            _storyUI.GetOnMaskWorldObservable().SelectMany(isOn => {
                if (isOn)
                {
                    return Observable.WhenAll(
                        _uiController.DeactivateHeaderAndFooter(),
                        _uiController.DarkenWorld()
                    );
                }
                else
                {
                    return Observable.WhenAll(
                        _uiController.ActivateHeaderAndFooter(),
                        _uiController.LightenWorld()
                    );
                }
            })
            .Subscribe().AddTo(_disposables);

            battleManager.GetOnEnterBeforeBossWaveStory()
            .SelectMany(story => _storyUI.Start(story))
            .Subscribe(_ => battleManager.ExitWaitBeforeBossWaitStory()).AddTo(_disposables);

            battleManager.GetOnEnterAfterBossDieStory()
            .SelectMany(story => _storyUI.Start(story))
            .Subscribe(_ => battleManager.ExitWaitAfterBossDieStory()).AddTo(_disposables);

            battleManager.GetOnEnterAfterBattleStory()
            .SelectMany(story => _storyUI.Start(story))
            .Subscribe(_ => battleManager.ExitWaitAfterBattleStory()).AddTo(_disposables);
        }
        protected override IObservable<VictoryOrDefeat> _activateBeforeResultUI(VictoryOrDefeat result)
        {
            // ストーリを持っているバトルの場合のみ、バトル後のストーリーを流そうと試みる。
            // ただし、バトル後ストーリーがあるとは限らない。
            var battleManager = (CpuBattleManager)SceneContainer.GetBattleManager();
            return battleManager.HasStory && result == VictoryOrDefeat.Win ? battleManager.EnterAfterBattleStory().Select(_ => result) : base._activateBeforeResultUI(result);
        }

        public override IObservable<IManager> LoadAssets(IFactory factory)
        {
            return base.LoadAssets(factory)
            .SelectMany(_ => _storyUI.LoadView())
            .Select(_ => (IManager)this);
        }

        public IObservable<bool> GetOnExitBeforeBattleStoryObservable()
        {
            return _onExitBeforeBattleStorySubject.AsObservable();
        }
        void _exitBeforeBattleStory()
        {
            _onExitBeforeBattleStorySubject.OnNext(true);
            _onExitBeforeBattleStorySubject.OnCompleted();
        }
    }
}
