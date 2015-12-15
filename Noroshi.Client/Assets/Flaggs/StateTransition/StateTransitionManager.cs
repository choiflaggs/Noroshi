using System;
using System.Collections.Generic;
using UniLinq;
using UniRx;

namespace Flaggs.StateTransition
{
    public sealed class StateTransitionManager<TState>
        where TState : class, IState
    {
        Dictionary<Type, TState> _states = new Dictionary<Type, TState>();
        Dictionary<Type, Type[]> _transitionMap = new Dictionary<Type, Type[]>();
        Subject<TState> _onEnterStateSubject = new Subject<TState>();
        Subject<TState> _onExitStateSubject = new Subject<TState>();
        TState _currentState;
        bool _isTransiting = false;


        public StateTransitionManager(Dictionary<Type, Type[]> transitionMap)
        {
            _transitionMap = transitionMap;
        }


        public void AddState<T>() where T : TState, new()
        {
            _states.Add(typeof(T), new T());
        }

        public IObservable<TState> GetOnEnterStateObservable()
        {
            return _onEnterStateSubject.AsObservable();
        }
        public IObservable<TState> GetOnExitStateObservable()
        {
            return _onExitStateSubject.AsObservable();
        }

        public void Start<T>() where T : TState
        {
            if (_currentState != null)
            {
                throw new InvalidOperationException();
            }
            _currentState = _getState<T>();
            _onEnterStateSubject.OnNext(_currentState);
        }
        public void Reset()
        {
            if (_currentState == null)
            {
                return;
            }
            _onExitStateSubject.OnNext(_currentState);
            _currentState = null;
        }

        public bool CurrentStateIs<T>() where T : TState
        {
            return _currentState != null && _currentState.GetType() == typeof(T);
        }

        public bool CanTransitTo<T>() where T : TState
        {
            if (_transitionMap.ContainsKey(_currentState.GetType()) && _transitionMap[_currentState.GetType()].Contains(typeof(T)))
            {
                return true;
            }
            return false;
        }

        public bool TransitTo<T>() where T : TState
        {
            // 遷移中の遷移割り込みサポートが望ましいが、
            // まだ未実装なので割り込み遷移で例外を投げて検知するという水際対応。
            if (_isTransiting)
            {
                throw new InvalidOperationException();
            }
            if (!CanTransitTo<T>())
            {
                return false;
            }
            _isTransiting = true;
            _onExitStateSubject.OnNext(_currentState);
            _currentState = _getState<T>();
            _onEnterStateSubject.OnNext(_currentState);
            _isTransiting = false;
            return true;
        }

        public TState GetCurrentState()
        {
            return _currentState;
        }

        TState _getState<T>() where T : TState
        {
            return _states[typeof(T)];
        }
    }
}
