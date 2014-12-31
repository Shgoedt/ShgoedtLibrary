using UnityEngine;
using System.Collections;

namespace Shgoedt.Interaction
{
	
	/// <summary>
	/// Interactable.
	/// Handles collission with the <see cref="Shgoedt.Interaction.InteractionHandler"/>.
	/// </summary>
	[RequireComponent( typeof( Collider ) )]
	[AddComponentMenu( "Interaction/Interactable" )]
	public class Interactable : MonoBehaviour
	{

		/// <summary>
		/// The interaction text.
		/// </summary>
		public string interactionText;
		
		#region Built-in Methods
		
		/// <summary>
		/// Awake this instance.
		/// </summary>
		void Awake()
		{
			// If there is no collider...
			if( collider == null )
			{
				// ... Log an error.
				Debug.LogError( string.Format( "No Collider attached to object \'{0}\'", name ) );
				// Abort method.
				return;
			}

			// Make sure the Interactable's Collider is a trigger.
			collider.isTrigger = true;

			// Set the correct layer.
			gameObject.layer = 8;
		}
		
		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update()
		{
			
		}
		
		#endregion

		#region Public Methods

		public virtual void Interact()
		{

		}

		#endregion
	}
}