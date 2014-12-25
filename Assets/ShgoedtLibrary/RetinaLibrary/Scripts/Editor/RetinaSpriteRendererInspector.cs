using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Shgoedt.Retina
{
	/// <summary>
	/// Custom Inspector for the RetinaSpriteRenderer class.
	/// </summary>
	[CustomEditor( typeof( RetinaSpriteRenderer ) )]
	public class RetinaSpriteRendererInspector : Editor
	{

		/// <summary>
		/// The retina sprite renderer.
		/// </summary>
		private RetinaSpriteRenderer retinaSpriteRenderer;

		/// <summary>
		/// Raised on the Inspector GUI event.
		/// </summary>
		public override void OnInspectorGUI()
		{
			// If the retinaRenderer is null...
			if( !retinaSpriteRenderer )
				// Assign target as RetinaRenderer.
				retinaSpriteRenderer = target as RetinaSpriteRenderer;

			// Add spacing.
			GUILayout.Space( 3f );

			// Add a field for the Sprite.
			retinaSpriteRenderer.retinaSprite = (Sprite)EditorGUILayout.ObjectField( "Retina Sprite", retinaSpriteRenderer.retinaSprite, typeof( Sprite ), true );

			// Space the button from the values.
			GUILayout.Space( 3f );

			// Add a button.
			if( GUILayout.Button( "Find Retina Sprite" ) )
				retinaSpriteRenderer.FindRetinaResource();
		}
	}
}