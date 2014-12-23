using UnityEngine;
using System.Collections;

namespace Shgoedt.StateMachine
{
	/// <summary>
	/// State class.
	/// </summary>
	public class State
	{

		/// <summary>
		/// The name of the <see cref="Shgoedt.StateMachine.State"/>.
		/// </summary>
		protected string name = "Empty";

		/// <summary>
		/// The owning <see cref="Shgoedt.StateMachine.StateMachine"/>.
		/// </summary>
		protected StateMachine owner;

		/// <summary>
		/// Initializes a new instance of the <see cref="Shgoedt.StateMachine.State"/> class.
		/// </summary>
		public State() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="Shgoedt.StateMachine.State"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		public State( string name )
		{
			this.name = name;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Shgoedt.StateMachine.State"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="owner">Owner.</param>
		public State( string name, StateMachine owner )
		{
			this.name = name;
			this.owner = owner;
		}

		/// <summary>
		/// Enter this <see cref="Shgoedt.StateMachine.State"/>.
		/// </summary>
		public virtual void Enter()
		{
			// Do intitialization.
		}

		/// <summary>
		/// Run this <see cref="Shgoedt.StateMachine.State"/>.
		/// </summary>
		public virtual void Run()
		{
			// Do behaviour.
		}

		/// <summary>
		/// Exit this <see cref="Shgoedt.StateMachine.State"/>.
		/// </summary>
		public virtual void Exit()
		{
			// Do exit.
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Shgoedt.StateMachine.State"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Shgoedt.StateMachine.State"/>.</returns>
		public override string ToString ()
		{
			return string.Format( "[State]{0}", name );
		}
	}
}
