using UnityEngine;
using System.Collections;

namespace Shgoedt
{
	/// <summary>
	/// Resolution class.
	/// </summary>
	public sealed class Resolution
	{

		/// <summary>
		/// Gets the aspect ratio.
		/// </summary>
		/// <returns>The aspect ratio.</returns>
		public static AspectRatio GetAspectRatio()
		{
			return new AspectRatio( Screen.width, Screen.height );
		}

		/// <summary>
		/// Gets the DPI.
		/// </summary>
		/// <returns>The DP.</returns>
		public static float GetDPI()
		{
			return Screen.dpi;
		}
	}

	/// <summary>
	/// Aspect ratio class.
	/// </summary>
	public class AspectRatio
	{
		/// <summary>
		/// The width.
		/// </summary>
		public float width;

		/// <summary>
		/// The height.
		/// </summary>
		public float height;

		public struct Ratio
		{
			/// <summary>
			/// A.
			/// </summary>
			public float a;
			/// <summary>
			/// The b.
			/// </summary>
			public float b;

			/// <summary>
			/// Returns a <see cref="System.String"/> that represents the current <see cref="Shgoedt.AspectRatio+ratio"/>.
			/// </summary>
			/// <returns>A <see cref="System.String"/> that represents the current <see cref="Shgoedt.AspectRatio+ratio"/>.</returns>
			public override string ToString()
			{
				return string.Format( "{0}:{1}", a, b );
			}
		}

		public Ratio ratio;

		/// <summary>
		/// Initializes a new instance of the <see cref="Shgoedt.AspectRatio"/> class.
		/// </summary>
		public AspectRatio() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="Shgoedt.AspectRatio"/> class.
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public AspectRatio( float width, float height )
		{
			this.width = width;
			this.height = height;
			this.ratio = CalculateAspectRatio();
		}

		Ratio CalculateAspectRatio()
		{
			Ratio output = new Ratio();
			output.a = width / height;
			output.b = 1;

			if( output.a == 1.78f )
				output.a = 16;

			return output;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Shgoedt.AspectRatio"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Shgoedt.AspectRatio"/>.</returns>
		public override string ToString()
		{
			return string.Format( "{0}:{1}", width / height, height );
		}
	}
}