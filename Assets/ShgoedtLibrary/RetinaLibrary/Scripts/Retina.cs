using UnityEngine;
using System.Collections;

namespace Shgoedt.Retina
{
	/// <summary>
	/// Retina support class.
	/// </summary>
	public sealed class Retina
	{

		/// <summary>
		/// Gets the content scale factor.
		/// </summary>
		/// <value>The content scale factor.</value>
		public static Vector2 ContentScaleFactor
		{
			get
			{
				// Return the ContentScaleFactor.
				return new Vector2( Screen.width / 480, Screen.height / 320 );
			}
		}

		/// <summary>
		/// The content scale threshold.
		/// </summary>
		private static int contentScaleThreshold = 2;

		/// <summary>
		/// Determines if is retina.
		/// </summary>
		/// <returns><c>true</c> if is retina; otherwise, <c>false</c>.</returns>
		public static bool IsRetina()
		{
			// If .x and .y of ContentScaleFactor is more then 1, return true.
			if( ContentScaleFactor.x >= contentScaleThreshold && ContentScaleFactor.y >= contentScaleThreshold )
			{
				Debug.Log( "Screen is retina! Content scale factor: " + ContentScaleFactor.ToString() );
				return true;
			}
			Debug.Log( "Screen is NOT retina! Content scale factor: " + ContentScaleFactor.ToString() );
			return false;
		}
	}
}