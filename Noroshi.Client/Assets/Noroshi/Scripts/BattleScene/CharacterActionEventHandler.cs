using UniRx;
using Noroshi.BattleScene.Actions;
using Noroshi.BattleScene.Actions.Attributes;
using Noroshi.BattleScene.CharacterEffect;
using Noroshi.BattleScene.Sound;
using Noroshi.Core.Game.Battle;

namespace Noroshi.BattleScene
{
    /// Character から切り出した ActionEvent を扱う機能をまとめたクラス。
    public class CharacterActionEventHandler
    {
        readonly ICharacter _character;
        AttributeHandler _attributeHandler;
        readonly Subject<int> _onAddDamageSubject = new Subject<int>();
        readonly Subject<CharacterEffectEvent> _onCommandCharacterEffectSubject = new Subject<CharacterEffectEvent>();
        readonly Subject<SoundEvent> _onCommandSoundSubject = new Subject<SoundEvent>();
        readonly Subject<SpecialEvent> _onSpecialEventSubject = new Subject<SpecialEvent>();
        readonly CompositeDisposable _disposables = new CompositeDisposable();
        readonly CompositeDisposable _attributeDisposables = new CompositeDisposable();

        public CharacterActionEventHandler(ICharacter character)
        {
            _character = character;
            _attributeHandler = new AttributeHandler(character);
            _attributeHandler.GetOnAddAttributeObservable().Subscribe(attribute =>
            {
                _commandCharacterEffect(attribute.CharacterEffectOncePlay ? CharacterEffectCommand.PlayOnce : CharacterEffectCommand.Play, attribute.CharacterEffectID);
            })
            .AddTo(_disposables);
            _attributeHandler.GetOnRemoveAttributeObservable().Subscribe(attribute =>
            {
                if (!attribute.CharacterEffectOncePlay) _commandCharacterEffect(CharacterEffectCommand.Stop, attribute.CharacterEffectID);
            })
            .AddTo(_disposables);
        }
        /// ActionEvent 経由でダメージを与える毎にダメージ数値がプッシュされる Observable を取得。
        public IObservable<int> GetOnAddDamageObservable() { return _onAddDamageSubject.AsObservable(); }
        /// ActionEvent 経由で CharacterEffect を操作する毎に命令がプッシュされる Observable を取得。
        public IObservable<CharacterEffectEvent> GetOnCommandCharacterEffectObservable() { return _onCommandCharacterEffectSubject.AsObservable(); }
        /// ActionEvent 経由で Sound を操作する毎に命令がプッシュされる Observable を取得。
        public IObservable<SoundEvent> GetOnCommandSoundObservable() { return _onCommandSoundSubject.AsObservable(); }
        /// ActionEvent 経由で SpecialEvent を送る毎にプッシュされる Observable を取得。
        public IObservable<SpecialEvent> GetOnSpecialEventObservable() { return _onSpecialEventSubject.AsObservable(); }
        /// シールド残割合がプッシュされる Observable を取得。
        public IObservable<ChangeableValueEvent> GetOnChangeShieldRatioObservable() { return _attributeHandler.GetOnChangeHPObservable(); }

        public IObservable<AttributeHandler> LoadData()
        {
            return _attributeHandler.LoadData();
        }

        /// Attribute を全て外す。
        public void RemoveAttributes()
        {
            _attributeDisposables.Clear();
            _attributeHandler.RemoveAttributes();
        }

        /// Action 実行者が ActionEvent を送る際の処理。
        public void SendActionEvent(ActionEvent actionEvent)
        {
            if (actionEvent.HPDamage.HasValue)
            {
                var damage = actionEvent.HPDamage.Value;
                if (damage > 0)
                {
                    _character.Energy.RecoverWhenSendDamage(damage, actionEvent.Target.MaxHP, actionEvent.Target.IsDead);
                    // HP 奪取
                    if (actionEvent.DamageType == DamageType.Physical)
                    {
                        var recoveryHp = damage * _character.LifeStealRating / (_character.LifeStealRating + actionEvent.Target.Level + 100) * 100;
                        _character.HP.RecoverWhenLifeSteal((uint)recoveryHp);
                    }
                    _onAddDamageSubject.OnNext(damage);
                }
            }
        }

        /// Action 対象者が ActionEvent を受ける際の処理。
        public TargetStateID? ReceiveActionEvent(ActionEvent actionEvent)
        {
            TargetStateID? stateId = null;
            _attributeHandler.ReceiveActionEvent(actionEvent);
            if (actionEvent.HPDamage.HasValue)
            {
                var damage = actionEvent.HPDamage.Value;
                if (damage > 0)
                {
                    // 与ダメージ係数があれば適用。
                    if (actionEvent.Executor != null && actionEvent.Executor.DamageCoefficient.HasValue) damage = (int)(damage * actionEvent.Executor.DamageCoefficient.Value);
                    _character.Energy.RecoverWhenReceiveDamage(damage, _character.HP.Max);
                }
                else
                {
                    // 回復上昇。
                    damage = (int)(damage * (1 + (float)_character.ImproveHealings / 100));
                }
                _character.HP.Damage(damage);
                if (actionEvent.TargetStateID.HasValue) stateId = actionEvent.TargetStateID.Value;

                if (actionEvent.HitCharacterEffectID.HasValue)
                {
                    var type = actionEvent.IsInterruptHitCharacterEffect ? CharacterEffectCommand.Interrupt : CharacterEffectCommand.PlayOnce;
                    _commandCharacterEffect(type, actionEvent.HitCharacterEffectID.Value);
                }
            }
            if (actionEvent.EnergyDamage.HasValue)
            {
                var damage = actionEvent.EnergyDamage.Value;
                _character.Energy.Damage(damage);

                if (actionEvent.HitCharacterEffectID.HasValue)
                {
                    var type = actionEvent.IsInterruptHitCharacterEffect ? CharacterEffectCommand.Interrupt : CharacterEffectCommand.PlayOnce;
                    _commandCharacterEffect(type, actionEvent.HitCharacterEffectID.Value);
                }
            }
            if (actionEvent.HitSoundID.HasValue)
            {
                _onCommandSoundSubject.OnNext(new SoundEvent(){
                    SoundID = actionEvent.HitSoundID.Value,
                    Command = SoundCommand.Play,
                });
            }
            if (actionEvent.Dodge)
            {
                _onSpecialEventSubject.OnNext(SpecialEvent.Dodge);
            }
            if (actionEvent.Attribute != null)
            {
                _attributeHandler.AddAttribute(actionEvent.Attribute)
                .Subscribe().AddTo(_attributeDisposables);
            }

            if (actionEvent.Executor != null)
            {
                actionEvent.Executor.SendActionEvent(actionEvent);
            }
            return stateId;
        }

        public bool HasMissDamage()
        {
            return _attributeHandler.HasMissDamage();
        }

        void _commandCharacterEffect(CharacterEffectCommand type, uint characterEffectId)
        {
            _onCommandCharacterEffectSubject.OnNext(new CharacterEffectEvent()
            {
                Command           = type,
                CharacterEffectID = characterEffectId,
            });
        }

        public interface ICharacter : IActionTarget
        {
            CharacterHP HP { get; }
            CharacterEnergy Energy { get; }
            uint LifeStealRating { get; }
            byte ImproveHealings { get; }
        }
    }
}
