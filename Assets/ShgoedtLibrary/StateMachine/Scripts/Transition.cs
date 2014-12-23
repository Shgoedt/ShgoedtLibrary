using UnityEngine;
using System.Collections;

namespace Shgoedt.StateMachine
{
	/// <summary>
	/// Transition class.
	/// </summary>
	public class Transition
	{
		/// <summary>
		/// The <see cref="Shgoedt.StateMachine.State"/> to transition from.
		/// </summary>
		State from;

		/// <summary>
		/// The <see cref="Shgoedt.StateMachine.State"/> to transition to.
		/// </summary>
		State to;

		/// <summary>
		/// The current <see cref="Shgoedt.StateMachine.State"/>.
		/// </summary>
		State currentState;

		/// <summary>
		/// Initializes a new instance of the <see cref="Shgoedt.StateMachine.Transition"/> class.
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <param name="name">Name.</param>
		public Transition( State from, State to )
		{
			this.from = from;
			this.to = to;
			currentState = from;
		}

		/// <summary>
		/// Progress this <see cref="Shgoedt.StateMachine.Transition"/>.
		/// </summary>
		public void Progress()
		{
			from.Exit();
			to.Enter();
			currentState = to;
		}

		/// <summary>
		/// Gets the current state.
		/// </summary>
		/// <returns>The state.</returns>
		public State CurrentState()
		{
			return currentState;
		}

		/// <summary>
		/// Gets the states.
		/// </summary>
		/// <returns>The states.</returns>
		public State[] GetStates()
		{
			return new State[2] { from, to };
		}
	}
}
