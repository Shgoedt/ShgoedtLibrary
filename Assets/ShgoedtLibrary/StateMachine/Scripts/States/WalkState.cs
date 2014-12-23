using UnityEngine;
using System.Collections;

namespace Shgoedt.StateMachine
{
	/// <summary>
	/// WalkState class.
	/// </summary>
	public class WalkState : State
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="Shgoedt.StateMachine.WalkState"/> class.
		/// </summary>
		public WalkState() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="Shgoedt.StateMachine.WalkState"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		public WalkState( string name )
		{
			this.name = name;
		}

		/// <summary>
		/// Enter this instance.
		/// </summary>
		public override void Enter()
		{
			base.Enter();
			// Set the name to "Walk".
			name = "Walk";
		}

		/// <summary>
		/// Run this instance.
		/// </summary>
		public override void Run()
		{
			base.Run();
		}

		/// <summary>
		/// Exit this instance.
		/// </summary>
		public override void Exit()
		{
			base.Exit();
		}
	}
}