using System;

namespace Noroshi.StateTransition
{
	public class State
	{
		public event EventHandler<StateEventArgs> OnEnter;
		public event EventHandler<StateEventArgs> OnExit;

		public virtual void Enter()
		{
			if (OnEnter != null) OnEnter(this, new StateEventArgs());
		}

		public virtual void Exit()
		{
			if (OnExit != null) OnExit(this, new StateEventArgs());
		}
	}
}
