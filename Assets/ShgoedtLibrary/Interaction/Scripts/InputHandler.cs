using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shgoedt.Interaction
{
	/// <summary>
	/// InputHandler class.
	/// </summary>
	public class InputHandler : MonoBehaviour
	{

		#region Properties

		/// <summary>
		/// The instance.
		/// </summary>
		internal static InputHandler instance;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static InputHandler Instance
		{
			get
			{
				if( instance == null )
				{
					InputHandler inputHandler = GameObject.Find( "InputHandler(Static)" ).GetComponent<InputHandler>();
					if( inputHandler == null )
						instance = new GameObject( "InputHandler(Static)", typeof( InputHandler ) ).GetComponent<InputHandler>();
					else
						instance = inputHandler;

					DontDestroyOnLoad( instance );
				}
				return instance;
			}
		}

		/// <summary>
		/// The selected target.
		/// </summary>
		public IClickable selectedTarget;

		/// <summary>
		/// The sphere casts.
		/// </summary>
		Collider2D[] sphereCasts = new Collider2D[5];
		/// <summary>
		/// The clickable targets.
		/// </summary>
		List<IClickable> clickableTargets = new List<IClickable>();

		#endregion

		/// <summary>
		/// Start this instance.
		/// </summary>
		void Start()
		{
			instance = this;
		}
		
		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update()
		{
			// If not pressing LMB
			if ( !Input.GetMouseButton(0) )
			{
				// Get the world position of the mouse.
				Vector3 pos = Camera.main.ScreenToWorldPoint( Input.mousePosition );

				// Null the selectedTarget.
				selectedTarget = null;
					
				// Clear the spherecasts and clickableTargets.
				for( int i = 0; i < sphereCasts.Length; ++i ) sphereCasts[i] = null;
				for( int i = 0; i < clickableTargets.Count; ++i ) clickableTargets[i].state = IClickableState.Normal;
				
				Physics2D.OverlapCircleNonAlloc( pos, .1f, sphereCasts, 1 << 9 ); //layer 9 is input colliders
				if ( sphereCasts != null )
				{
					clickableTargets.Clear();
					
					foreach( Collider2D col in sphereCasts )
					{
						if( col != null )
						{
							MonoBehaviour[] components = col.GetComponents<MonoBehaviour>();
							for( int i = 0; i < components.Length; ++i )
							{
								IClickable iClickable = components[i] as IClickable;
								if( iClickable != null && iClickable.CanUse() )
								{
									iClickable.state = IClickableState.Hover;
									clickableTargets.Add( iClickable );
								}
							}
						}
					}
					
					float distance = float.MaxValue;
					Vector3 optionPos;
					foreach( IClickable target in clickableTargets )
					{
						optionPos = target.Position;
						optionPos.z = pos.z;
						if ( Vector3.Distance( optionPos, pos ) < distance )
						{
							//Debug.Log( target );
							selectedTarget = target;
							distance = Vector3.Distance( optionPos, pos );
							
						}
					}
				}
			}

			if( selectedTarget != null )
			{
				if( Input.GetMouseButton( 0 ) )
					selectedTarget.Pressed();
			}
		}
	}
}