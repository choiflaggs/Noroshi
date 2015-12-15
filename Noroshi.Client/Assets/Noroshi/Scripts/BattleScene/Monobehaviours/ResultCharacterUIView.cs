using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using DG.Tweening;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class ResultCharacterUIView : UIView 
    {
        [SerializeField] Image _characterImage;
        [SerializeField] Text _levelText;
        [SerializeField] Text _statisticsText;
        [SerializeField] GaugeView _statisticsGauge;
        [SerializeField] GaugeView _expGauge;
        [SerializeField] Image _expGaugeFullEffectImage;
        [SerializeField] Image _expLevelupEffectImage;

        readonly float _progressionSpeed = 1.0f;

        RectTransform _levelupImageRectTransform = null;
        Vector2 _defaultLevelupImagePosition = Vector2.zero;
        uint _damage = 0;
        uint _nowDamage = 0;
        float _damageRate = 0.0f;
        float _currentExpRatio = 0.0f;
        ushort _levelUpNum = 0;

        CharacterThumbnail _characterThumbnail;

        new void Awake()
        {
            base.Awake();
            if (_expLevelupEffectImage != null)
            {
                _levelupImageRectTransform = _expLevelupEffectImage.rectTransform;
                _defaultLevelupImagePosition = _levelupImageRectTransform.anchoredPosition;
            }
        }

        void Start()
        {
            _statisticsGauge.SetActive(false);
            _statisticsText.text = "0";
            _statisticsText.enabled = false;
            if (_expGauge != null) _expGauge.SetActive(false);
        }

        public void SetCharacterThumbnail(CharacterThumbnail characterThumbnail)
        {
            _characterThumbnail = characterThumbnail;

            _levelText.text = "Lv " + characterThumbnail.Level;
            // TODO : サムネイル画像反映。
            // TODO : EvolutionLevel と PromotionLevel の反映。
            if (characterThumbnail.IsDead)
            {
                _characterImage.color = Color.gray;
                // 死んでいれば経験値スライダーは隠してしまう。
                if (_expGauge != null) _expGauge.SetActive(false);//.gameObject.SetActive(false);
            }
        }
        public IObservable<Sprite> LoadSprite()
        {
            if (_characterThumbnail == null) return Observable.Return<Sprite>(null);
            var factory = (Factory)SceneContainer.GetFactory();
            return factory.BuildCharacterThumbSprite(_characterThumbnail.CharacterID, _characterThumbnail.SkinLevel)
            .Do(s => _characterImage.sprite = s);
        }

        public void SetProgress(float previousExpRatio, float currentExpRatio, ushort levelUpNum)
        {
            _currentExpRatio = currentExpRatio;
            _levelUpNum = levelUpNum;

            _expGauge.SetValue(previousExpRatio);
        }

        public void SetStatistics(uint damage, float damageRatio)
        {
            _damage = damage;
            _damageRate = damageRatio;
            _statisticsGauge.SetValue(0.0f);
        }

        public IObservable<ResultCharacterUIView> PlaySliderAnimation()
        {
            var expGaugeAnimationObservable = Observable.Return<bool>(false);
            if (_expGauge != null && !_characterThumbnail.IsDead)
            {
                _expGauge.SetActive(true);
                var sumExpRate = _currentExpRatio + (1.0f * _levelUpNum);
                expGaugeAnimationObservable = _expGauge.SetAnimationValue(sumExpRate, _progressionSpeed, true)
                    .Do(isNext => 
                    {
                        if (isNext)
                        {
                            _playImageAlphaAnimation(_expGaugeFullEffectImage, 0.0f, 1.0f, 0.3f);
                            _playImageAlphaAnimation(_expLevelupEffectImage, 0.0f, 1.0f, 0.3f);
                            _playImageAnchorMoveAnimation(_levelupImageRectTransform, _defaultLevelupImagePosition, 
                                                      new Vector2(_defaultLevelupImagePosition.x, _defaultLevelupImagePosition.y + 15.0f), 0.3f);
                        }
                    });
            }
            _statisticsText.enabled = true;
            _statisticsGauge.SetActive(true);
            var statisticsGaugeAnimationObservable = _statisticsGauge.SetAnimationValue(_damageRate, _progressionSpeed, true);

            this.UpdateAsObservable()
                .Do(_ => 
                    {
                    if (_damage == 0) return;
                    // GaugeViewのValue進行量とテキスト表示部分の進行量を同期させる
                    _nowDamage += (uint)((float)_damage * ((Time.deltaTime * _progressionSpeed) / _damageRate));
                    if (_nowDamage > _damage)
                    {
                        _nowDamage = _damage;
                    }
                    _statisticsText.text = _nowDamage.ToString();
                })
                .TakeWhile(_ => _nowDamage < _damage)
                .Subscribe();

            return Observable.WhenAll(
                statisticsGaugeAnimationObservable,
                expGaugeAnimationObservable).Select(_ => this);
        }

        void _playImageAlphaAnimation(Image image, float initialAlpha, float endAlpha, float duration)
        {
            if (image == null) return;
            image.DOKill();
            image.color = new Color(image.color.r, image.color.g, image.color.b, initialAlpha);
            var endColor = new Color(image.color.r, image.color.g, image.color.b, endAlpha);
            image.DOColor(endColor, duration).SetLoops(2, LoopType.Yoyo);
        }

        void _playImageAnchorMoveAnimation(RectTransform rectTransform, Vector2 initialAnchorPosition, Vector2 endAnchorPosition, float duration)
        {
            if (rectTransform == null) return;
            rectTransform.DOKill();
            rectTransform.anchoredPosition = initialAnchorPosition;
            rectTransform.DOAnchorPos(endAnchorPosition, duration);
        }
    }
}