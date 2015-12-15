using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Noroshi.BattleScene;
using UniRx;
using Noroshi.Core.Game.Character;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class StatusBoostIconView : UIView
    {
        enum StatusBoostTypes
        {
            StrengthUp = 0,
            StrengthDown,
            IntellectUp,
            IntellectDown,
            AgilityUp,
            AgilityDown,
            PhysucalAttackUp,
            PhysucalAttackDown,
            MagicPowerUp,
            MagicPowerDown,
            ArmorUp,
            ArmorDown,
            MagicRegistanceUp,
            MagicRegistanceDown,
            AccuracyUp,
            AccuracyDown,
            DodgeUp,
            DodgeDown,
            LifeStealRatingUp,
            LifeStealRatingDown,
            ActionFrequencyUp,
            ActionFrequencyDown,
            MaxHp
        }

        [System.Serializable]
        class StatusBoostIconSprite
        {
            [SerializeField] Sprite _upIcon;
            [SerializeField] Sprite _downIcon;

            public Sprite UpIcon { get { return _upIcon; } }
            public Sprite DownIcon { get { return _downIcon; } }
        }

        [SerializeField] Image _statusBoostIconImage;
        [SerializeField] Image _commonIconEffectImage;
        [SerializeField] StatusBoostIconSprite _strengthIcon;
        [SerializeField] StatusBoostIconSprite _intellectIcon;
        [SerializeField] StatusBoostIconSprite _agilityIcon;
        [SerializeField] StatusBoostIconSprite _physucalAttackIcon;
        [SerializeField] StatusBoostIconSprite _magicPowerIcon;
        [SerializeField] StatusBoostIconSprite _armorIcon;
        [SerializeField] StatusBoostIconSprite _magicRegistanceIcon;
        [SerializeField] StatusBoostIconSprite _accuracyIcon;
        [SerializeField] StatusBoostIconSprite _dodgeIcon;
        [SerializeField] StatusBoostIconSprite _lifeStealRating;
        [SerializeField] StatusBoostIconSprite _actionFrequencyIcon;
        [SerializeField] StatusBoostIconSprite _maxHpIcon;

        List<StatusBoostTypes> _statusBoostTypes = null;
        Animator _commonIconAnimator;
        AnimatorStateInfo _commonAnimatorStateInfo = default(AnimatorStateInfo);
        System.IDisposable _changeStatusBoostIconDisposable = null;

        new void Awake()
        {
            base.Awake();
            _statusBoostTypes = new List<StatusBoostTypes>();
            _commonIconAnimator = _commonIconEffectImage.GetComponent<Animator>();
            _commonAnimatorStateInfo = _commonIconAnimator.GetCurrentAnimatorStateInfo(0);
            _commonIconEffectImage.enabled = false;
            _statusBoostIconImage.enabled = false;
        }

        void OnDestroy()
        {
            if (_changeStatusBoostIconDisposable != null) _changeStatusBoostIconDisposable.Dispose();
            _statusBoostTypes.Clear();
            _statusBoostTypes = null;
        }

        public void SetStatusBoostEvent(CharacterStatusBoostEvent.EventType characterStatusBoostEventType, IStatusBoostFactor statusBoostFactor)
        {
            if (_changeStatusBoostIconDisposable != null) _changeStatusBoostIconDisposable.Dispose();
            if (_statusBoostTypes == null) return;
            if (characterStatusBoostEventType == CharacterStatusBoostEvent.EventType.Add)
            {
                _addAllStatusBoostTypes(statusBoostFactor);
                if (_statusBoostTypes.Count > 0) _playCommonIconEffectAnimation();
            }
            else if (characterStatusBoostEventType == CharacterStatusBoostEvent.EventType.Remove)
            {
                _removeAllStatusBoostType(statusBoostFactor);
            }

            if(_statusBoostTypes.Count == 0)
            {
                _commonIconEffectImage.enabled = false;
                _statusBoostIconImage.enabled = false;
                return;
            }

            var iconIndex = _statusBoostTypes.Count - 1;
            _statusBoostIconImage.sprite = _getStatusBoostIcon(_statusBoostTypes[iconIndex]);
            _changeStatusBoostIconDisposable = Observable.Interval(System.TimeSpan.FromSeconds(1.0))
                .Subscribe(_ => 
                {
                    iconIndex--;
                    if (iconIndex < 0) iconIndex = _statusBoostTypes.Count - 1;
                    _statusBoostIconImage.sprite = _getStatusBoostIcon(_statusBoostTypes[iconIndex]);
                });
        }

        void _addAllStatusBoostTypes(IStatusBoostFactor addFactor)
        {
            _addOneStatusBoostType(addFactor.Strength, StatusBoostTypes.StrengthUp, StatusBoostTypes.StrengthDown);
            _addOneStatusBoostType(addFactor.Intellect, StatusBoostTypes.IntellectUp, StatusBoostTypes.IntellectDown);
            _addOneStatusBoostType(addFactor.Agility, StatusBoostTypes.AgilityUp, StatusBoostTypes.AgilityDown);
            _addOneStatusBoostType(addFactor.PhysicalAttack, StatusBoostTypes.PhysucalAttackUp, StatusBoostTypes.PhysucalAttackDown);
            _addOneStatusBoostType(addFactor.MagicPower, StatusBoostTypes.MagicPowerUp, StatusBoostTypes.MagicPowerDown);
            _addOneStatusBoostType(addFactor.Armor, StatusBoostTypes.ArmorUp, StatusBoostTypes.ArmorDown);
            _addOneStatusBoostType(addFactor.MagicRegistance, StatusBoostTypes.MagicRegistanceUp, StatusBoostTypes.MagicRegistanceDown);
            _addOneStatusBoostType(addFactor.Accuracy, StatusBoostTypes.AccuracyUp, StatusBoostTypes.AccuracyDown);
            _addOneStatusBoostType(addFactor.Dodge, StatusBoostTypes.DodgeUp, StatusBoostTypes.DodgeDown);
            _addOneStatusBoostType(addFactor.LifeStealRating, StatusBoostTypes.LifeStealRatingUp, StatusBoostTypes.LifeStealRatingDown);
            _addOneStatusBoostType(addFactor.ActionFrequency, StatusBoostTypes.ActionFrequencyUp, StatusBoostTypes.ActionFrequencyDown);
            _addOneStatusBoostType(addFactor.MaxHp, StatusBoostTypes.MaxHp, StatusBoostTypes.MaxHp);
        }

        void _addOneStatusBoostType(int statusParameter, StatusBoostTypes statusBoostTypeUp, StatusBoostTypes statusBoostTypeDown)
        {
            if (statusParameter == 0) return;
            var statusBoostType = (statusParameter > 0) ? statusBoostTypeUp : statusBoostTypeDown;
            if (!_statusBoostTypes.Contains(statusBoostType))
            {
                _statusBoostTypes.Add(statusBoostType);
            }
            else
            {
                _updateStatusBoostType(statusBoostType);
            }
        }

        void _addOneStatusBoostType(float actionFrequency, StatusBoostTypes statusBoostTypeUp, StatusBoostTypes statusBoostTypeDown)
        {
            if (actionFrequency == 0.0f) return;
            var statusBoostType = (actionFrequency > 0.0f) ? StatusBoostTypes.ActionFrequencyUp : StatusBoostTypes.ActionFrequencyDown;
            if (!_statusBoostTypes.Contains(statusBoostType))
            {
                _statusBoostTypes.Add(statusBoostType);
            }
            else
            {
                _updateStatusBoostType(statusBoostType);
            }
        }

        void _updateStatusBoostType(StatusBoostTypes statusBoostType)
        {
            // すでに付与されているステータス増減に対し上書きされた時
            // 現在いる位置から末尾に持っていく
            // その上で追加アニメーションを再生させる
            var _updateIndex = _statusBoostTypes.IndexOf(statusBoostType);
            if (_updateIndex < 0) return;
            _statusBoostTypes.RemoveAt(_updateIndex);
            _statusBoostTypes.Add(statusBoostType);
        }

        void _removeAllStatusBoostType(IStatusBoostFactor removeFactor)
        {
            _removeOneStatusBoostType(removeFactor.Strength, StatusBoostTypes.StrengthUp, StatusBoostTypes.StrengthDown);
            _removeOneStatusBoostType(removeFactor.Intellect, StatusBoostTypes.IntellectUp, StatusBoostTypes.IntellectDown);
            _removeOneStatusBoostType(removeFactor.Agility, StatusBoostTypes.AgilityUp, StatusBoostTypes.AgilityDown);
            _removeOneStatusBoostType(removeFactor.PhysicalAttack, StatusBoostTypes.PhysucalAttackUp, StatusBoostTypes.PhysucalAttackDown);
            _removeOneStatusBoostType(removeFactor.MagicPower, StatusBoostTypes.MagicPowerUp, StatusBoostTypes.MagicPowerDown);
            _removeOneStatusBoostType(removeFactor.Armor, StatusBoostTypes.ArmorUp, StatusBoostTypes.ArmorDown);
            _removeOneStatusBoostType(removeFactor.MagicRegistance, StatusBoostTypes.MagicRegistanceUp, StatusBoostTypes.MagicRegistanceDown);
            _removeOneStatusBoostType(removeFactor.Accuracy, StatusBoostTypes.AccuracyUp, StatusBoostTypes.AccuracyDown);
            _removeOneStatusBoostType(removeFactor.Dodge, StatusBoostTypes.DodgeUp, StatusBoostTypes.DodgeDown);
            _removeOneStatusBoostType(removeFactor.LifeStealRating, StatusBoostTypes.LifeStealRatingUp, StatusBoostTypes.LifeStealRatingDown);
            _removeOneStatusBoostType(removeFactor.ActionFrequency, StatusBoostTypes.ActionFrequencyUp, StatusBoostTypes.ActionFrequencyDown);
            _removeOneStatusBoostType(removeFactor.MaxHp, StatusBoostTypes.MaxHp, StatusBoostTypes.MaxHp);
        }

        void _removeOneStatusBoostType(int statusParameter, StatusBoostTypes statusBoostTypeUp, StatusBoostTypes statusBoostTypeDown)
        {
            if (statusParameter == 0) return;
            var statusBoostType = (statusParameter > 0) ? statusBoostTypeUp : statusBoostTypeDown;
            if (_statusBoostTypes.Contains(statusBoostType)) _statusBoostTypes.Remove(statusBoostType);
        }

        void _removeOneStatusBoostType(float actionFrequency, StatusBoostTypes statusBoostTypeUp, StatusBoostTypes statusBoostTypeDown)
        {
            if (actionFrequency == 0.0f) return;
            var statusBoostType = (actionFrequency > 0.0f) ? statusBoostTypeUp : statusBoostTypeDown;
            if (_statusBoostTypes.Contains(statusBoostType)) _statusBoostTypes.Remove(statusBoostType);
        }

        void _playCommonIconEffectAnimation()
        {
            _commonIconEffectImage.enabled = true;
            _statusBoostIconImage.enabled = true;
            if (!_commonIconAnimator.enabled)
            {
                _commonIconAnimator.enabled = true;
            }
            else
            {
                _commonIconAnimator.Play(_commonAnimatorStateInfo.shortNameHash, 0, 0);
            }
        }

        Sprite _getStatusBoostIcon(StatusBoostTypes statusBoostTypes)
        {
            switch (statusBoostTypes)
            {
            case StatusBoostTypes.StrengthUp:
                return _strengthIcon.UpIcon;
            case StatusBoostTypes.StrengthDown:
                return _strengthIcon.DownIcon;
            case StatusBoostTypes.IntellectUp:
                return _intellectIcon.UpIcon;
            case StatusBoostTypes.IntellectDown:
                return _intellectIcon.DownIcon;
            case StatusBoostTypes.AgilityUp:
                return _agilityIcon.UpIcon;
            case StatusBoostTypes.AgilityDown:
                return _agilityIcon.DownIcon;
            case StatusBoostTypes.PhysucalAttackUp:
                return _physucalAttackIcon.UpIcon;
            case StatusBoostTypes.PhysucalAttackDown:
                return _physucalAttackIcon.DownIcon;
            case StatusBoostTypes.MagicPowerUp:
                return _magicPowerIcon.UpIcon;
            case StatusBoostTypes.MagicPowerDown:
                return _magicPowerIcon.DownIcon;
            case StatusBoostTypes.ArmorUp:
                return _armorIcon.UpIcon;
            case StatusBoostTypes.ArmorDown:
                return _armorIcon.DownIcon;
            case StatusBoostTypes.MagicRegistanceUp:
                return _magicRegistanceIcon.UpIcon;
            case StatusBoostTypes.MagicRegistanceDown:
                return _magicRegistanceIcon.DownIcon;
            case StatusBoostTypes.AccuracyUp:
                return _accuracyIcon.UpIcon;
            case StatusBoostTypes.AccuracyDown:
                return _accuracyIcon.DownIcon;
            case StatusBoostTypes.DodgeUp:
                return _dodgeIcon.UpIcon;
            case StatusBoostTypes.DodgeDown:
                return _dodgeIcon.DownIcon;
            case StatusBoostTypes.LifeStealRatingUp:
                return _lifeStealRating.UpIcon;
            case StatusBoostTypes.LifeStealRatingDown:
                return _lifeStealRating.DownIcon;
            case StatusBoostTypes.ActionFrequencyUp:
                return _actionFrequencyIcon.UpIcon;
            case StatusBoostTypes.ActionFrequencyDown:
                return _actionFrequencyIcon.DownIcon;
            case StatusBoostTypes.MaxHp:
                return _maxHpIcon.UpIcon;
            default:
                return null;
            }
        }
    }
}