using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Shgoedt.StateMachine;
#if UNITY_4_6
using UnityEngine.UI;
#endif

/// <summary>
/// StateMachineDemo class.
/// </summary>
public class StateMachineDemo : MonoBehaviour
{

	public StateMachine stateMachine;

	int stateMachineRunHash = 0, stateMachineWalkHash;

	bool running = false;

#if UNITY_4_6
	public Text StateMachineInfo;
#endif

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
#if UNITY_4_6
		if( !StateMachineInfo )
			StateMachineInfo = Object.FindObjectOfType<Text>();
#endif
		StateMachine.onStateChanged += UpdateInfo;

		stateMachineRunHash = StateMachine.StringToHash( "Run" );
		stateMachineWalkHash = StateMachine.StringToHash( "Walk" );
		
		stateMachine = new StateMachine();

		stateMachine.AddTransition( new WalkState( "Walk" ), new RunState( "Run" ), "Run" );
		stateMachine.AddTransition( new RunState( "Run"), new WalkState( "Walk" ), "Walk" );
		stateMachine.Initialize( new WalkState( "Walk" ) );

	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{

		stateMachine.Update();

		if( Input.GetKeyDown( KeyCode.S ) )
			stateMachine.ChangeState( "Break" );

		if( Input.GetKeyDown( KeyCode.Space ) )
		{
			if( running )
				stateMachine.ChangeState( stateMachineWalkHash );
			else
				stateMachine.ChangeState( stateMachineRunHash );

			running = !running;
		}
	}

	void UpdateInfo()
	{
		string debugText = "";

		debugText = "<b>StateMachine Info</b>:\r\n";
		debugText += string.Format( "Current State: {0}\r\n", stateMachine.state.ToString() );
		debugText += "\r\n";
		
		foreach( KeyValuePair<int, Transition> transition in stateMachine.transitions )
		{
			debugText += string.Format( "Transition {0}:\r\n", transition.Key );
			debugText += string.Format( "\t{0} -> {1}\r\n", transition.Value.GetStates()[0].ToString(), transition.Value.GetStates()[1].ToString() );
		}

#if UNITY_4_6
		StateMachineInfo.text = debugText;
#else
		GUI.Label( new Rect( 20, 20, 300, 1000 ), debugText );
#endif
	}
}
