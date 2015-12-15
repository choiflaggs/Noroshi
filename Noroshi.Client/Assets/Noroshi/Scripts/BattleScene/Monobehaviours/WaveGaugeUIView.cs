using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using Noroshi.BattleScene.UI;
using Noroshi.Core.Game.Battle;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class WaveGaugeUIView : UIView, IWaveGaugeUIView
    {
        [SerializeField] GaugeView _hpGaugeView = null;
        [SerializeField] Sprite _rarityNormalSprite;
        [SerializeField] Sprite _raritySpecialSprite;
        [SerializeField] Image _rarityImage;
        [SerializeField] Text _enemyLvAndName;
        [SerializeField] float _duration = 0.75f;
        IDisposable _hpGaugeDisposable;

        new void Awake()
        {
            base.Awake();
            SetActive(false);
        }

        public void Initialize(byte level, string waveGuageTextKey, float hpRatio, WaveGaugeType waveGaugeType)
        {
            SetActive(true);
            _hpGaugeView.SetValue(hpRatio);
            _enemyLvAndName.text = string.Format(waveGuageTextKey, level);
            _rarityImage.sprite = waveGaugeType == WaveGaugeType.NormalRaidBoss ? _rarityNormalSprite : _raritySpecialSprite;
        }
        
        public void ChangeHpRatio(float raito)
        {
            if (_hpGaugeDisposable != null)
            {
                _hpGaugeDisposable.Dispose();
                _hpGaugeView.ResetAnimationEndValue();
            }
            _hpGaugeView.SetFillAlertValue(raito);
            _hpGaugeDisposable = _hpGaugeView.SetAnimationValue(raito, _duration, false).Subscribe();
        }

        void OnDestroy()
        {
            base.Dispose();
            if (_hpGaugeDisposable != null) _hpGaugeDisposable.Dispose();
        }
    }
}