using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Noroshi.BattleScene.UI;

using DG.Tweening;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class OwnCharacterPanelUIView : UIView, IOwnCharacterPanelUIView
    {
        enum FrameSpriteTypes
        {
            NormalFrame = 0,
            AlertFrame,
            DeadFrame
        }

        enum SliderEnergySpriteTypes
        {
            SkillNormal = 0,
            Dead
        }

        enum SliderHpSpriteTypes
        {
            HpAlert1 = 0,
            HpAlert2,
            HpNormal
        }

        enum HpFontTypes
        {
            Normal = 0,
            Alert
        }

        [SerializeField] Image _characterImage;
        [SerializeField] Image _skillMaxEffectImage;
        [Header("---通常時と死亡時を表示する")]
        [SerializeField] Image _normalFrameImage;
        [Header("---瀕死時を表示する")]
        [SerializeField] Image _alertEffectFrameImage;
        [Header("---スキルMax時の明滅用")]
        [SerializeField] Image _skillMaxEffectFrameImage;

        [SerializeField] GaugeView _hpGaude;
        [SerializeField] GaugeView _energyGauge;

        [SerializeField] RectTransform _moveTransform;

        [SerializeField] UIView _skillEffectGroupView;

        [SerializeField] ImageSquareMoveUI _frameLight1Animation;
        [SerializeField] ImageSquareMoveUI _frameLight2Animation;

        [SerializeField] Text _skillLvName;
        [SerializeField] Text _hpText = null;

        [SerializeField] SkillStartingEffectAnimation _skillStartingEffectAnimation = null;

        [SerializeField] StatusBoostIconView _statusBoostIconView;

        [SerializeField] Font[] _hpFonts;

        [Header("---交換するスプライト")]
        [SerializeField] Sprite[] _changeFrameSprites;
        [SerializeField] Sprite[] _changeHpSliderSprites;
        [SerializeField] Sprite[] _changeEnergySliderSprites;

        readonly float _buttonMoveValue = 35.0f;
        readonly float _movueDuration = 0.25f;
        readonly float _alphaAnimDuration = 0.5f;
        readonly float _hpAlertValue = 0.3f;
        Subject<bool> _onClickSubject = new Subject<bool>();

        Image _hpSliderImage = null;
        Image _hpSliderFillEffectImage = null;
        Image _energySliderImage = null;
        Image _energySliderFillEffectImage = null;

        Vector2 _defaultAnchorPos;
        Color _endColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        bool _isAvailable = false;
        bool _isInvocationActiveAction = false;
        bool _isDrawSkillEffectAnimation = true;


        new void Awake()
        {
            base.Awake();
            _hpText.text = "0";

            _setSkillMaxEffectImageActive(false);

            ToggleActiveActionAvailable(false);

            _defaultAnchorPos = _moveTransform.anchoredPosition;
            _hpSliderImage = _hpGaude.FillImage;
            _hpSliderFillEffectImage = _hpGaude.FillAlertImage;
            _energySliderImage = _energyGauge.FillImage;
            _energySliderFillEffectImage = _energyGauge.FillAlertImage;
        }

        void Start()
        {
            _setSkillEffectGroupViewActive(false);

            _hpGaude.GetOnValueChangeObservalbe()
                .Select(n => n <= _hpAlertValue && n > 0.0f)
                .DistinctUntilChanged()
                .Subscribe(flg => { _setupHpSpriteAnimation(_hpSliderImage, flg); })
                .AddTo(_hpGaude);
        }

        void _setupHpSpriteAnimation(Image image, bool flg)
        {
            _hpSliderFillEffectImage.enabled = flg;
            _alertEffectFrameImage.enabled = flg;
            if (flg)
            {
                image.sprite = _changeHpSliderSprites[(int)SliderHpSpriteTypes.HpAlert1];
                _imageAlphaAnimationPlay(_hpSliderFillEffectImage);
                _imageAlphaAnimationPlay(_alertEffectFrameImage);

                _hpText.font = _hpFonts[(int)HpFontTypes.Alert];
            }
            else
            {
                image.sprite = _changeHpSliderSprites[(int)SliderHpSpriteTypes.HpNormal];
                _imageAlphaAnimationKill(_hpSliderFillEffectImage);
                _imageAlphaAnimationKill(_alertEffectFrameImage);
                
                _hpText.font = _hpFonts[(int)HpFontTypes.Normal];
            }
        }
        
        void _setupEnergySpriteAnimation(bool available)
        {
            if (_energySliderFillEffectImage == null) return;
            _setSkillEffectGroupViewActive(available);
            _setSkillMaxEffectImageActive(available);
            if (available)
            {
                _moveTransform.DOAnchorPos(new Vector2(_defaultAnchorPos.x, _defaultAnchorPos.y + _buttonMoveValue), _movueDuration).SetEase(Ease.OutBack);

                _imageAlphaAnimationPlay(_skillMaxEffectFrameImage);

                _frameLight1Animation.AnimationPlay();
                _frameLight2Animation.AnimationPlay();
            }
            else
            {
                if (!_isInvocationActiveAction) _moveDownButton();

                _imageAlphaAnimationKill(_skillMaxEffectFrameImage);
                _frameLight1Animation.AnimationPause();
                _frameLight2Animation.AnimationPause();
            }
        }

        void _imageAlphaAnimationPlay(Image image)
        {
            _imageAlphaAnimationKill(image);
            image.DOColor(_endColor, _alphaAnimDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        void _imageAlphaAnimationKill(Image image)
        {
            image.DOKill();
            image.color = Color.white;
        }

        void _setSkillMaxEffectImageActive(bool isActive)
        {
            _skillMaxEffectImage.gameObject.SetActive(isActive);
        }

        void _setSkillEffectGroupViewActive(bool isActive)
        {
            _skillEffectGroupView.SetActive(isActive);
        }

        void _setDeadSprite(bool isDead)
        {
            if (isDead)
            {
                _setupEnergySpriteAnimation(false);
                _characterImage.color = Color.gray;
            }
            else
            {
                _characterImage.color = Color.white;
            }

            _energySliderImage.sprite = isDead ? _changeEnergySliderSprites[(int)SliderEnergySpriteTypes.Dead] : _changeEnergySliderSprites[(int)SliderEnergySpriteTypes.SkillNormal];
            _normalFrameImage.sprite = isDead ? _changeFrameSprites[(int)FrameSpriteTypes.DeadFrame] : _changeFrameSprites[(int)FrameSpriteTypes.NormalFrame];
        }

        void _moveDownButton()
        {
            _moveTransform.DOAnchorPos(_defaultAnchorPos, _movueDuration).SetEase(Ease.InBack);
        }

        public IObservable<bool> GetOnClickObservable()
        {
            return _onClickSubject.AsObservable();
        }

        public void ChangeHP(ChangeableValueEvent hpEvent)
        {
            _hpText.text = hpEvent.Current.ToString();
            _hpGaude.SetValue((float)hpEvent.Current / hpEvent.Max);
            var isDead = _hpGaude.Value <= 0.0f;
            _setDeadSprite(isDead);
            if (isDead)
            {
                _hpText.color = Color.gray;
                _imageAlphaAnimationKill(_hpSliderFillEffectImage);
                _moveDownButton();
            }
        }
        public void ChangeEnergy(ChangeableValueEvent energyEvent)
        {
            var nowValue = (float)energyEvent.Current / energyEvent.Max;
            _energyGauge.SetValue(nowValue);

            bool isEnergyMax = nowValue >= 1.0f;
            _energySliderFillEffectImage.enabled = isEnergyMax;
            if (isEnergyMax)
            {
                _imageAlphaAnimationPlay(_energySliderFillEffectImage);
            }
            else
            {
                _imageAlphaAnimationKill(_energySliderFillEffectImage);
            }
        }

        public void ToggleActiveActionAvailable(bool available)
        {
            if (available != _isAvailable) 
            {
                if (available && _isDrawSkillEffectAnimation)
                {
                    _isDrawSkillEffectAnimation = false;
                    _skillStartingEffectAnimation.PlayAnimation();
                }
                _isAvailable = available;
                _setupEnergySpriteAnimation(_isAvailable);
            }
        }
        
        public void Initialize(int activeActionLevel, uint initializeHp, uint maxHp, int initializeEnergy, int maxEnergy)
        {
            _hpText.text = initializeHp.ToString();
            _hpGaude.SetValue((float)initializeHp / maxHp);
            _energyGauge.SetValue((float)initializeEnergy / maxEnergy);
            _skillLvName.text = string.Format(_skillLvName.text, activeActionLevel);
        }

        public void ChangeStatusBoost(CharacterStatusBoostEvent boostEvent)
        {
            _statusBoostIconView.SetStatusBoostEvent(boostEvent.Type, boostEvent.StatusBoosterFactor);
        }

        public void FinishActiveAction()
        {
            _isInvocationActiveAction = false;
            _isDrawSkillEffectAnimation = true;
            _moveDownButton();
        }

        public void SetSprite(Sprite sprite)
        {
            _characterImage.sprite = sprite;
        }

        public void EnterActiveAction()
        {
            _isInvocationActiveAction = true;
        }

        /// クリックメソッド。UI から結びつけられる。
        public void Click()
        {
            _onClickSubject.OnNext(true);
        }
    }
}
