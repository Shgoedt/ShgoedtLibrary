using UnityEngine;
using System.Collections;

namespace Shgoedt.StateMachine
{
	/// <summary>
	/// RunState class.
	/// </summary>
	public class RunState : State
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="Shgoedt.StateMachine.RunState"/> class.
		/// </summary>
		public RunState() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="Shgoedt.StateMachine.RunState"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		public RunState( string name )
		{
			this.name = name;
		}

		/// <summary>
		/// Enter this instance.
		/// </summary>
		public override void Enter()
		{
			base.Enter();
			// Set the name to "Run".
			name = "Run";
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