using UnityEngine;
using System.Collections;
using Shgoedt.Events;

/// <summary>
/// MessengerDemo class.
/// </summary>
public class MessengerDemo : MonoBehaviour
{

	/// <summary>
	/// The type.
	/// </summary>
	public string type;

	/// <summary>
	/// The color.
	/// </summary>
	public Color myColor;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		Messenger.AddListener( this.gameObject, "CharacterCustomized", "ChangeColor" );
		Messenger.AddListener( this.gameObject, "LevelLoaded", "ChangeType" );

		Invoke( "BroadcastColor", 2f );
		Invoke( "BroadcastType", 3f );
	}

	void Broadcast()
	{
		Messenger.Broadcast( "CharacterCustomized" );
	}

	void BroadcastType()
	{
		Messenger.Broadcast( "LevelLoaded" );
	}

	void ChangeColor()
	{
		myColor = Color.red;
	}

	void ChangeType()
	{
		type = "NewType";
	}
}
