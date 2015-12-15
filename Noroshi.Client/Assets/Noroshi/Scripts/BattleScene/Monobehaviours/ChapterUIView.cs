using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Noroshi.BattleScene.UI;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class ChapterUIView : UIView, IChapterUIView
    {
        [SerializeField] Text _chapterText;
        [SerializeField] Text _titleText;
        [SerializeField] float _activeTime = 2.5f;
        [SerializeField] float _deactivateDuration = 0.5f;
        [SerializeField] float _chapterTextActivateDuration = 1f;

        Image _image;
        Text[] _texts;

        new void Awake()
        {
            base.Awake();
            _image = GetComponent<Image>();
            _texts = GetComponentsInChildren<Text>();
            SetActive(false);
        }
        public IObservable<IChapterUIView> Activate(string titleTextKey)
        {
            SetActive(true);
            var onDeactivate = new Subject<IChapterUIView>();
            var delay = _activeTime - _deactivateDuration;
            _image.DOColor(new Color(0, 0, 0, 0), _deactivateDuration)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                SetActive(false);
                onDeactivate.OnNext(this);
                onDeactivate.OnCompleted();
            });
            for (var i = 0; i < _texts.Length; i++)
            {
                _texts[i].DOColor(new Color(0, 0, 0, 0), _deactivateDuration).SetDelay(delay);
            }
            var chapterTextColor = _chapterText.color;
            var prevChapterTextColor = new Color(chapterTextColor.r, chapterTextColor.g, chapterTextColor.b, 0);
            _chapterText.color = prevChapterTextColor;
            _chapterText.DOColor(chapterTextColor, _chapterTextActivateDuration);
            _chapterText.text = titleTextKey;
            _titleText.text = GlobalContainer.LocalizationManager.GetText(titleTextKey);
            return onDeactivate.AsObservable();
        }
    }
}
