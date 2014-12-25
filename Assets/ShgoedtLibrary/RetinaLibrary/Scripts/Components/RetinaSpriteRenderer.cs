using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace Shgoedt.Retina
{
	/// <summary>
	/// RetinaSpriteRenderer class.
	/// </summary>
	[AddComponentMenu("Rendering/Sprite Renderer (Retina)")]
	[RequireComponent( typeof( SpriteRenderer ) )]
	public class RetinaSpriteRenderer : RetinaRenderer
	{

		/// <summary>
		/// The retina sprite.
		/// </summary>
		public Sprite retinaSprite;
		
		/// <summary>
		/// Gets the sprite renderer.
		/// </summary>
		/// <value>The sprite renderer.</value>
		private SpriteRenderer spriteRenderer
		{
			get
			{
				// Return the renderer as a SpriteRenderer.
				return renderer as SpriteRenderer;
			}
		}

		/// <summary>
		/// Awake this instance.
		/// </summary>
		public override void Awake ()
		{
			// Call base.Awake()
			base.Awake();

			// If there is no Retina Sprite specified...
			if( retinaSprite == null )
			{
				FindRetinaResource();
			}
			else
			{
				// If the Screen is Retina...
				if( Retina.IsRetina() )
				{
					// ... set the sprite to be the retina sprite.
					spriteRenderer.sprite = retinaSprite;
				}
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Finds the retina resource.
		/// </summary>
		public override void FindRetinaResource()
		{
			base.FindRetinaResource();

			// If there is no Sprite assigned in the SpriteRenderer component.
			if( spriteRenderer.sprite == null )
			{
				Debug.LogWarning( string.Format( "No Sprite assigned in the SpriteRenderer component on object \"{0}\"", spriteRenderer.name ) );
				return;
			}

			// If there is already a Retina Sprite assigned in the SpriteRenderer component.
			if( spriteRenderer.sprite.name.Contains( "@2x" ) )
			{
				Debug.LogWarning( string.Format( "Suspected Retina Sprite already assigned in the SpriteRenderer component on object \"{0}\"", spriteRenderer.name ) );
				return;
			}

			// Cache the path to the sprite
			string spritePath = AssetDatabase.GetAssetPath( spriteRenderer.sprite );

			// Create a memory block for the retina path.
			string retinaPath = string.Empty;

			// If we can get the asset path to the name...
			if( spritePath.Length > 0 )
			{
				// ... find a retina variant. (e.g. "button.png" -> "button@2x.png")

				// Store the split spritePath.
				string[] spritePathSplit = spritePath.Split( new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries );

				// Add "@2x" between "button" and "png".
				retinaPath = spritePathSplit[0] + "@2x." + spritePathSplit[1];

				// Set the retina sprite.
				retinaSprite = (Sprite)AssetDatabase.LoadAssetAtPath( retinaPath, typeof( Sprite ) );
			}
		}
#endif
	}
}