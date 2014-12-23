using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shgoedt.Events
{
	/// <summary>
	/// Messenger class.
	/// </summary>
	public sealed class Messenger
	{
		
		/// <summary>
		/// The subscribed listeners.
		/// </summary>
		internal readonly static Dictionary<string, List<Listener>> eventTable = new Dictionary<string, List<Listener>>();
		
		/// <summary>
		/// Adds the listener.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="eventName">Event name.</param>
		/// <param name="callback">Callback.</param>
		public static void AddListener( GameObject target, string onEvent, string callback )
		{
			// Create the new listener.
			Listener listenerToAdd = new Listener( target, callback );

			// If the event name is not registered...
			if( !eventTable.ContainsKey( onEvent ) )
				// Create a new List of Listeners.
				eventTable.Add( onEvent, new List<Listener>() );

			// Add it to the list of Listeners.
			eventTable[onEvent].Add( listenerToAdd );
		}

		/// <summary>
		/// Removes the listener.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="onEvent">On event.</param>
		/// <param name="callback">Callback.</param>
		public static void RemoveListener( GameObject target, string onEvent, string callback )
		{
			// If the eventTable has the onEvent as key...
			if( eventTable.ContainsKey( onEvent ) )
				// ... compare every Listener in the list...
				for( int i = 0; i < eventTable[onEvent].Count; ++i )
				{
					// ... if the Listener matches...
					if( eventTable[onEvent][i].target == target && eventTable[onEvent][i].callback == callback )
						// ... remove the Listener.
						eventTable[onEvent].RemoveAt( i );
				}
		}

		/// <summary>
		/// Broadcast the specified event name.
		/// </summary>
		/// <param name="eventName">Event name.</param>
		public static void Broadcast( string eventName )
		{
			if( !eventTable.ContainsKey( eventName ) ) return;

			// SendMessage to every Listener for that Event.
			for( int i = 0; i < eventTable[eventName].Count; i++ )
			{
				// SendMessage.
				eventTable[eventName][i].target.SendMessage( eventTable[eventName][i].callback, SendMessageOptions.DontRequireReceiver );
			}
		}
		/// <summary>
		/// Broadcast the specified event name and value.
		/// </summary>
		/// <param name="eventName">Event name.</param>
		/// <param name="value">Value.</param>
		public static void Broadcast( string eventName, object value )
		{
			if( !eventTable.ContainsKey( eventName ) ) return;

			// SendMessage to every Listener for that Event.
			for( int i = 0; i < eventTable[eventName].Count; ++i )
			{
				// SendMessage.
				eventTable[eventName][i].target.SendMessage( eventTable[eventName][i].callback, value, SendMessageOptions.DontRequireReceiver );
			}
		}
	}

	/// <summary>
	/// Listener class.
	/// </summary>
	internal class Listener
	{
		/// <summary>
		/// The target.
		/// </summary>
		public GameObject target;
		/// <summary>
		/// The callback method.
		/// </summary>
		public string callback;

		/// <summary>
		/// Initializes a new instance of the <see cref="Shgoedt.Events.Listener"/> class.
		/// </summary>
		public Listener() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="Shgoedt.Events.Listener"/> class.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="onEvent">On event.</param>
		/// <param name="callback">Callback.</param>
		public Listener( GameObject target, string callback )
		{
			this.target = target;
			this.callback = callback;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Shgoedt.Events.Listener"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Shgoedt.Events.Listener"/>.</returns>
		public override string ToString ()
		{
			return string.Format( "Listener for {0}.{1}()", target.name, callback );
		}
	}
}