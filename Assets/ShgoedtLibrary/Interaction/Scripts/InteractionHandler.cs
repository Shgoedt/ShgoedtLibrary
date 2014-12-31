using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Shgoedt.Interaction
{

	/// <summary>
	/// Interaction Handler.
	/// Cast rays to the nearest Interactable.
	/// </summary>
	public class InteractionHandler : MonoBehaviour
	{

		#region Properties

		/// <summary>
		/// The Camera from which the <see cref="Shgoedt.Interaction.InteractionHandler"/> will cast rays.
		/// </summary>
		public Camera interactionCamera;

		/// <summary>
		/// The interaction range.
		/// </summary>
		public float interactionRange;

		/// <summary>
		/// The interaction label.
		/// </summary>
		public Text interactionLabel;

		/// <summary>
		/// The center of the screen.
		/// </summary>
		Vector3 centerOfScreen
		{
			get
			{
				return new Vector3( Screen.width / 2, Screen.height / 2, 0 );
			}
		}

		/// <summary>
		/// The selected interactable.
		/// </summary>
		Interactable selectedInteractable;

		/// <summary>
		/// The raycast.
		/// </summary>
		Ray ray;

		/// <summary>
		/// The ray's hit info.
		/// </summary>
		RaycastHit rayHit;

		#endregion

		#region Built-in Methods
		
		/// <summary>
		/// Awake this instance.
		/// </summary>
		void Awake()
		{
			// If no interaction camera is specified...
			if( interactionCamera == null )
				// ... set the Main Camera as the interaction Camera.
				interactionCamera = ( Camera.main != null ) ? Camera.main : FindObjectOfType<Camera>();

			if( interactionLabel == null )
				Debug.LogError( "Please create a UnityEngine.UI.Text component!" );
		}
		
		/// <summary>
		/// Update this instance at Time.fixedTime.
		/// </summary>
		void FixedUpdate()
		{
			// Null the selected interactable.
			selectedInteractable = null;

			// Set the Raycast.
			ray = interactionCamera.ViewportPointToRay( new Vector3( 0.5f, 0.5f, 0 ) );   

			// If we hit a Collider on Layer 8 (Interactable)...
			if( Physics.Raycast( ray, out rayHit, interactionRange, 1 << 8 ) )
			{
				// ... Set the selected Interactable to be that object.
				selectedInteractable = rayHit.collider.GetComponent<Interactable>();
			}

			if( selectedInteractable != null )
				interactionLabel.text = selectedInteractable.interactionText;

		}
		
		#endregion
	}
}