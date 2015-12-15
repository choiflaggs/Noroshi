using System;
using System.Collections.Generic;
using UniLinq;
using UniRx;
using Flaggs.StateTransition;
using Noroshi.BattleScene.CharacterEffect;
using Noroshi.Core.Game.Battle;

namespace Noroshi.BattleScene.CharacterState
{
    public class CharacterStateHandler
    {
        static readonly Dictionary<Type, Type[]> CHARACTER_STATE_MAP = new Dictionary<Type, Type[]>
        {
            {typeof(AliveState), new[]{typeof(DeadState), typeof(WinState)}},
        };
        static readonly Dictionary<Type, Type[]> ALIVE_STATE_MAP = new Dictionary<Type, Type[]>
        {
            {typeof(NormalState), new[]{typeof(ActiveActionState), typeof(InterruptState), typeof(ApparentDeathState)}},
            {typeof(ActiveActionState), new[]{typeof(NormalState), typeof(InterruptState), typeof(ApparentDeathState)}},
            {typeof(InterruptState), new[]{typeof(NormalState), typeof(ApparentDeathState)}},
            {typeof(ApparentDeathState), new[]{typeof(NormalState)}},
        };
        static readonly Dictionary<Type, Type[]> NORMAL_STATE_MAP = new Dictionary<Type, Type[]>
        {
            {typeof(IdleState), new[]{typeof(ProceedState), typeof(ActionState)}},
            {typeof(ProceedState), new[]{typeof(IdleState), typeof(ActionState)}},
            {typeof(ActionState), new[]{typeof(IdleState),  typeof(ActionState)}},
        };
        static readonly Dictionary<Type, Type[]> INTERRUPT_STATE_MAP = new Dictionary<Type, Type[]>
        {
            {typeof(DamageState), new[]{typeof(KnockbackState), typeof(StopState)}},
            {typeof(KnockbackState), new[]{typeof(StopState)}},
            {typeof(StopState), new[]{typeof(StopState)}},
        };


        StateTransitionManager<CharacterState> _characterStateTransitionManager = new StateTransitionManager<CharacterState>(CHARACTER_STATE_MAP);
        StateTransitionManager<CharacterState> _aliveStateTransitionManager = new StateTransitionManager<CharacterState>(ALIVE_STATE_MAP);
        StateTransitionManager<CharacterState> _normalStateTransitionManager = new StateTransitionManager<CharacterState>(NORMAL_STATE_MAP);
        StateTransitionManager<CharacterState> _interruptStateStateTransitionManager = new StateTransitionManager<CharacterState>(INTERRUPT_STATE_MAP);

        ICharacterView _characterView;
        string _animationName;
        Subject<CharacterEffectEvent> _onChangeEffectSubject = new Subject<CharacterEffectEvent>();
        CompositeDisposable _disposables = new CompositeDisposable();
        IDisposable _timeoutDisposable = null;

        public CharacterStateHandler()
        {
            _characterStateTransitionManager.AddState<AliveState>();
            _characterStateTransitionManager.AddState<DeadState>();
            _characterStateTransitionManager.AddState<WinState>();

            _aliveStateTransitionManager.AddState<NormalState>();
            _aliveStateTransitionManager.AddState<ActiveActionState>();
            _aliveStateTransitionManager.AddState<InterruptState>();
            _aliveStateTransitionManager.AddState<ApparentDeathState>();

            _normalStateTransitionManager.AddState<IdleState>();
            _normalStateTransitionManager.AddState<ProceedState>();
            _normalStateTransitionManager.AddState<ActionState>();

            _interruptStateStateTransitionManager.AddState<DamageState>();
            _interruptStateStateTransitionManager.AddState<KnockbackState>();
            _interruptStateStateTransitionManager.AddState<StopState>();

            GetOnExitStateObservable().Where(state => state.GetType() == typeof(AliveState))
                .Subscribe(_ => _aliveStateTransitionManager.Reset()).AddTo(_disposables);
            GetOnExitStateObservable().Where(state => state.GetType() == typeof(NormalState))
                .Subscribe(_ => _normalStateTransitionManager.Reset()).AddTo(_disposables);
            GetOnExitStateObservable().Where(state => state.GetType() == typeof(InterruptState))
                .Subscribe(_ => _interruptStateStateTransitionManager.Reset()).AddTo(_disposables);

            // アニメーション設定
            var typeToAnimationActionMap = new Dictionary<Type, System.Action>(){
                {typeof(IdleState)        , () => _characterView.PlayIdle()},
                {typeof(ProceedState)     , () => _characterView.PlayWalk()},
                {typeof(DamageState)      , () => _characterView.PlayDamage()},
                {typeof(KnockbackState)   , () => _characterView.PlayDamage()},
                {typeof(StopState)        , () => _characterView.PlayStun()},
                {typeof(ActionState)      , () => _characterView.PlayAction(_animationName)},
                {typeof(ActiveActionState), () => _characterView.PlayAction(_animationName)},
                {typeof(WinState)         , () => _characterView.PlayWin()},
            };
            GetOnEnterStateObservable().Where(state => typeToAnimationActionMap.ContainsKey(state.GetType()))
                .Subscribe(state => typeToAnimationActionMap[state.GetType()].Invoke()).AddTo(_disposables);
            // 状態遷移時は必ず移動を止めておく
            GetOnExitStateObservable()
                .Subscribe(_=> _characterView.StopMove()).AddTo(_disposables);
            // 時間制限付き状態
            var typeToStateTimeMap = new Dictionary<Type, float>(){
                {typeof(DamageState)   , Constant.DAMAGE_TIME},
                {typeof(KnockbackState), Constant.KNOCKBACK_TIME},
            };
            GetOnEnterStateObservable().Where(state => typeToStateTimeMap.ContainsKey(state.GetType()))
                .Subscribe(state => {
                    _timeoutDisposable = SceneContainer.GetTimeHandler().Timer(typeToStateTimeMap[state.GetType()])
                        .Subscribe(_ => TryToTransitToIdle());
                }).AddTo(_disposables);
            GetOnExitStateObservable().Where(state => typeToStateTimeMap.ContainsKey(state.GetType()))
                .Subscribe(state => _timeoutDisposable.Dispose()).AddTo(_disposables);
        }

        public bool CanLogicUpdate() { return _characterStateTransitionManager.CurrentStateIs<AliveState>(); }
        public bool IsTargetable() { return !IsDead() && !IsApparentDeath(); }

        public void Start()
        {
            _characterStateTransitionManager.Start<AliveState>();
            _aliveStateTransitionManager.Start<NormalState>();
            _normalStateTransitionManager.Start<IdleState>();
        }
        public void Reset()
        {
            if (IsDead()) return;
            ForceReset();
        }
        public void ForceReset()
        {
            _characterStateTransitionManager.Reset();
            _aliveStateTransitionManager.Reset();
            _normalStateTransitionManager.Reset();
            _interruptStateStateTransitionManager.Reset();
        }
        public IObservable<CharacterEffectEvent> GetOnCommandCharacterEffectObservable()
        {
            return _onChangeEffectSubject.AsObservable();
        }
        
        public void SetCharacterView(ICharacterView view)
        {
            _characterView = view;
        }
        
        public bool IsDead()
        {
            return _characterStateTransitionManager.CurrentStateIs<DeadState>();
        }
        public bool IsApparentDeath()
        {
            if (!_characterStateTransitionManager.CurrentStateIs<AliveState>()) return false;
            return _aliveStateTransitionManager.CurrentStateIs<ApparentDeathState>();
        }
        public bool IsAction()
        {
            if (!_characterStateTransitionManager.CurrentStateIs<AliveState>()) return false;
            if (_aliveStateTransitionManager.GetCurrentState() != null &&
                _aliveStateTransitionManager.GetCurrentState().IsActionState) return true;
            if (_normalStateTransitionManager.GetCurrentState() != null &&
                _normalStateTransitionManager.GetCurrentState().IsActionState) return true;
            return false;
        }
        public bool CanChargeNormalActionGauge()
        {
            if (!_characterStateTransitionManager.CurrentStateIs<AliveState>()) return false;
            if (!_aliveStateTransitionManager.CurrentStateIs<NormalState>()) return false;
            return _normalStateTransitionManager.CurrentStateIs<IdleState>() || 
                   _normalStateTransitionManager.CurrentStateIs<ProceedState>();
        }

        public IObservable<CharacterState> GetOnEnterStateObservable()
        {
            return _getStateTransitionManagers().Select(stm => stm.GetOnEnterStateObservable()).Merge();
        }
        public IObservable<CharacterState> GetOnExitStateObservable()
        {
            return _getStateTransitionManagers().Select(stm => stm.GetOnExitStateObservable()).Merge();
        }
        public IObservable<CharacterState> GetOnEnterActionObservable()
        {
            return GetOnEnterStateObservable().Where(state => state.IsActionState);
        }
        public IObservable<CharacterState> GetOnExitActionObservable()
        {
            return GetOnExitStateObservable().Where(state => state.IsActionState);
        }

        public bool CanTransitToAction()
        {
            if (!_characterStateTransitionManager.CurrentStateIs<AliveState>()) return false;
            if (!_aliveStateTransitionManager.CurrentStateIs<NormalState>()) return false;
            return _normalStateTransitionManager.CanTransitTo<ActionState>();
        }

        public bool TryToTransitToIdle()
        {
            if (!_characterStateTransitionManager.CurrentStateIs<AliveState>()) return false;
            if (_aliveStateTransitionManager.CurrentStateIs<NormalState>())
            {
                return _normalStateTransitionManager.TransitTo<IdleState>();
            }
            else if (_aliveStateTransitionManager.TransitTo<NormalState>())
            {
                _normalStateTransitionManager.Start<IdleState>();
                return true;
            }
            return false;
        }
        public bool TryToTransitToProceed()
        {
            if (!_characterStateTransitionManager.CurrentStateIs<AliveState>()) return false;
            if (!_aliveStateTransitionManager.CurrentStateIs<NormalState>()) return false;
            return _normalStateTransitionManager.TransitTo<ProceedState>();
        }
        public bool TryToTransitToAction(string animationName)
        {
            _animationName = animationName;
            if (!_characterStateTransitionManager.CurrentStateIs<AliveState>()) return false;
            if (!_aliveStateTransitionManager.CurrentStateIs<NormalState>()) return false;
            return _normalStateTransitionManager.TransitTo<ActionState>();
        }

        public bool TryToTransitToActiveAction(string animationName)
        {
            _animationName = animationName;
            if (!_characterStateTransitionManager.CurrentStateIs<AliveState>()) return false;
            return _aliveStateTransitionManager.TransitTo<ActiveActionState>();
        }

        public bool TryToTransitToApparentDeath()
        {
            if (!_characterStateTransitionManager.CurrentStateIs<AliveState>()) return false;
            return _aliveStateTransitionManager.TransitTo<ApparentDeathState>();
        }

        public bool TryToTransitToDamage()
        {
            return _tryToTransitToInterruptState<DamageState>();
        }
        public bool TryToTransitToKnockback()
        {
            return _tryToTransitToInterruptState<KnockbackState>();
        }
        public bool TryToTransitToStop()
        {
            return _tryToTransitToInterruptState<StopState>();
        }
        bool _tryToTransitToInterruptState<T>() where T : CharacterState
        {
            if (!_characterStateTransitionManager.CurrentStateIs<AliveState>()) return false;
            if (_aliveStateTransitionManager.CurrentStateIs<InterruptState>())
            {
                return _interruptStateStateTransitionManager.TransitTo<T>();
            }
            else if (_aliveStateTransitionManager.TransitTo<InterruptState>())
            {
                _interruptStateStateTransitionManager.Start<T>();
                return true;
            }
            return false;
        }

        public bool TryToTransitToDead()
        {
            return _characterStateTransitionManager.TransitTo<DeadState>();
        }
        public bool TransitToWin()
        {
            return _characterStateTransitionManager.TransitTo<WinState>();
        }
        
        public bool TryToTransitByStateID(TargetStateID stateId)
        {
            if (stateId == TargetStateID.Damage)
            {
                return TryToTransitToDamage();
            }
            else if (stateId == TargetStateID.Knockback)
            {
                return TryToTransitToKnockback();
            }
            return false;
        }

        IEnumerable<StateTransitionManager<CharacterState>> _getStateTransitionManagers()
        {
            return new []
            {
                _characterStateTransitionManager,
                _aliveStateTransitionManager,
                _normalStateTransitionManager,
                _interruptStateStateTransitionManager,
            };
        }

        void _commandCharacterEffect(CharacterEffectCommand command, uint characterEffectId)
        {
            _onChangeEffectSubject.OnNext(new CharacterEffectEvent()
            {
                Command           = command,
                CharacterEffectID = characterEffectId,
            });
        }

        public class AliveState : CharacterState
        {
        }
        public class DeadState : CharacterState
        {
        }
        public class WinState : CharacterState
        {
        }
        
        public class NormalState : CharacterState
        {
        }
        public class ActiveActionState : CharacterState
        {
            public override bool IsActionState { get { return true; } }
        }
        public class InterruptState : CharacterState
        {
        }
        public class ApparentDeathState : CharacterState
        {
            public override bool IsActionState { get { return true; } }
        }
        
        public class IdleState : CharacterState
        {
        }
        public class ProceedState : CharacterState
        {
        }
        public class ActionState : CharacterState
        {
            public override bool IsActionState { get { return true; } }
        }
        
        public class DamageState : CharacterState
        {
        }
        public class KnockbackState : CharacterState
        {
        }
        public class StopState : CharacterState
        {
        }
    }
}
