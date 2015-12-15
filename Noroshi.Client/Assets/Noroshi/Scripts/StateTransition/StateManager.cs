using System;
using System.Collections.Generic;
using UniRx;

namespace Noroshi.StateTransition
{
    public class StateManager
    {
        protected State _currentState;
        bool _isTransiting = false;
        Dictionary<Type, State> _states = new Dictionary<Type, State>();
        Subject<Type> _onEnterStateSubject = new Subject<Type>();
        Subject<Type> _onExitStateSubject = new Subject<Type>();

        public IObservable<Type> GetOnEnterStateObservable()
        {
            return _onEnterStateSubject.AsObservable();
        }
        public IObservable<Type> GetOnExitStateObservable()
        {
            return _onExitStateSubject.AsObservable();
        }

        public void Start<T>() where T : State
        {
            _currentState = _getState<T>();
            _onEnterStateSubject.OnNext(_currentState.GetType());
            _currentState.Enter();
        }
        public void Finish()
        {
            _currentState.Exit();
            _onExitStateSubject.OnNext(_currentState.GetType());
            _currentState = null;
        }

        public void TransitTo<T>() where T : State
        {
            // 遷移中の遷移割り込みサポートが望ましいが、
            // まだ未実装なので割り込み遷移で例外を投げて検知するという水際対応。
            if (_isTransiting)
            {
                throw new InvalidOperationException();
            }
            _isTransiting = true;
            _currentState.Exit();
            _onExitStateSubject.OnNext(_currentState.GetType());
            _currentState = _getState<T>();
            _onEnterStateSubject.OnNext(_currentState.GetType());
            _currentState.Enter();
            _isTransiting = false;
        }
        protected bool _tryToTransit<T>(Predicate<State> canTransit) where T : State
        {
            if (canTransit(_currentState))
            {
                TransitTo<T>();
                return true;
            }
            return false;
        }

        protected T _getState<T>() where T : State
        {
            return (T)_states[typeof(T)];
        }
        public void AddState(State state)
        {
            _states.Add(state.GetType(), state);
        }

        public void AddOnEnter<T>(EventHandler<StateEventArgs> onEnter) where T : State
        {
            _getState<T>().OnEnter += onEnter;
        }
        public void AddOnExit<T>(EventHandler<StateEventArgs> onExit) where T : State
        {
            _getState<T>().OnExit += onExit;
        }
    }
}