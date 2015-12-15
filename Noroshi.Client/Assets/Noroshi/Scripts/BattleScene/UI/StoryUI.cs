using UniLinq;
using UniRx;

namespace Noroshi.BattleScene.UI
{
    public class StoryUI : UIViewModel<IStoryUIView>
    {
        Subject<bool> _onMaskWorldSubject = new Subject<bool>();

        public StoryUI()
        {
        }
        protected override IObservable<IStoryUIView> _loadView()
        {
            return SceneContainer.GetFactory().BuildStoryUIView();
        }

        public IObservable<bool> GetOnMaskWorldObservable()
        {
            return _onMaskWorldSubject.AsObservable();
        }

        public IObservable<bool> Start(Story story)
        {
            _onMaskWorldSubject.OnNext(true);
            return _uiView.Begin()
            .SelectMany(_ =>
            {
                story.GoNextMessage();
                return _uiView.GoNext(story.GetCurrentMessageOwnCharacterView(), story.GetCurrentMessageEnemyCharacterView(), story.GetCurrentMessageTextKey(), story.GetCurrentCharacterNameTextKey());
            })
            .Buffer(story.GetMessageNum() + 1)
            .SelectMany(_ => _uiView.Finish())
            .Do(_ => _onMaskWorldSubject.OnNext(false))
            .Select(_ => true);
        }
    }
}
