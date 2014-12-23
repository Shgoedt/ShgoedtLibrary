using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shgoedt.StateMachine
{

	public enum StateMachineError
	{
		TransitionDoesNotExist,
		HashDoesNotExist,
	};

	/// <summary>
	/// StateMachine class.
	/// </summary>
	public class StateMachine
	{

		#region Delegates

		/// <summary>
		/// The OnStateChanged delegate.
		/// </summary>
		public delegate void OnStateChanged();
		/// <summary>
		/// The OnStateChanged delegate method.
		/// </summary>
		public static OnStateChanged onStateChanged;

		#endregion


		#region Variables

		/// <summary>
		/// The current state.
		/// </summary>
		public State state;

		/// <summary>
		/// The transitions.
		/// </summary>
		public Dictionary<int, Transition> transitions = new Dictionary<int, Transition>();

		#endregion


		#region Initializers

		/// <summary>
		/// Initializes a new instance of the <see cref="Shgoedt.StateMachine.StateMachine"/> class.
		/// </summary>
		public StateMachine()
		{
			// Initialize
		}

		#endregion


		#region Public methods

		/// <summary>
		/// Initialize the <see cref="Shgoedt.StateMachine.StateMachine"/>.
		/// </summary>
		public void Initialize() { Initialize( null ); }
		/// <summary>
		/// Initialize the <see cref="Shgoedt.StateMachine.StateMachine"/> with the specified <see cref="Shgoedt.StateMachine.State"/>.
		/// </summary>
		/// <param name="initialState">Initial state.</param>
		public void Initialize( State initialState )
		{
			this.state = initialState;
			onStateChanged();
		}

		/// <summary>
		/// Update this <see cref="Shgoedt.StateMachine.StateMachine"/>.
		/// </summary>
		public void Update()
		{
			// If the state is not null.
			if( state != null )
				// Run the current state.
				state.Run();
		}

		/// <summary>
		/// Adds a transition between two states.
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <param name="name">Name.</param>
		public void AddTransition( State from, State to, string name )
		{
			// Create a new Transition.
			Transition transition = new Transition( from, to );
			// Assign the new Transition indexed by name.
			transitions.Add( name.GetHashCode(), transition );
		}

		/// <summary>
		/// Removes the transition.
		/// </summary>
		/// <param name="name">Name.</param>
		public void RemoveTransition( string name ) { RemoveTransition( name.GetHashCode() ); }
		/// <summary>
		/// Removes the transition.
		/// </summary>
		/// <param name="hash">Hash.</param>
		public void RemoveTransition( int hash )
		{
			// If the name of the transition doesn't exist, throw an error.
			if( !transitions.ContainsKey( hash ) )
			{
				ThrowError( StateMachineError.TransitionDoesNotExist, hash );
				// Abort method.
				return;
			}

			// Remove the Transition.
			transitions.Remove( hash );
		}

		/// <summary>
		/// Changes the state.
		/// </summary>
		/// <param name="transition">Transition.</param>
		public void ChangeState( string name )
		{
			// If the name of the transition doesn't exist, throw an error.
			if( !transitions.ContainsKey( name.GetHashCode() ) )
			{
				ThrowError( StateMachineError.TransitionDoesNotExist, name, name.GetHashCode() );
				// Abort method.
				return;
			}

			// Change the state.
			ChangeState( name.GetHashCode() );
		}
		/// <summary>
		/// Changes the state.
		/// </summary>
		/// <param name="hash">Hash.</param>
		public void ChangeState( int hash )
		{
			// Progress the Transition
			transitions[hash].Progress();
			
			// Assign the new state.
			state = transitions[hash].CurrentState();

			// Call the onStateChanged delegate functions.
			onStateChanged();
		}

		#endregion


		#region Static methods

		/// <summary>
		/// Throws the error.
		/// </summary>
		/// <param name="error">Error.</param>
		/// <param name="args">Arguments.</param>
		public static void ThrowError( StateMachineError error, params object[] args )
		{
			string msg = string.Empty;

			switch( error )
			{
			case StateMachineError.TransitionDoesNotExist:
				msg += string.Format( "Transition with name \"{0}\" and hash ({1}) does not exist.", args[0], args[1] );
				break;
			case StateMachineError.HashDoesNotExist:
				msg += string.Format( "Hashcode for name \"{0}\" does not exist.", args[0] );
				break;
			default:
				msg += "No StateMachineError was passed!";
				break;
			}

			Debug.LogError( msg );
		}

		/// <summary>
		/// Return the HashCode of the string.
		/// </summary>
		/// <returns>The to hash.</returns>
		/// <param name="name">Name.</param>
		public static int StringToHash( string name )
		{
			// Return the hashcode of the string.
			return name.GetHashCode();
		}

		#endregion
	}
}

