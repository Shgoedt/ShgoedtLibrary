using UnityEngine;
using System.Collections;

namespace Shgoedt
{
	/// <summary>
	/// Math class.
	/// </summary>
	public sealed class Math
	{
		/// <summary>
		/// Map the specified value from ( a, b ) to ( c, d ).
		/// </summary>
		/// <param name="n">Value.</param>
		/// <param name="a">A minimum.</param>
		/// <param name="b">A maximum.</param>
		/// <param name="c">B minimum.</param>
		/// <param name="d">B maximum.</param>
		public static float Map( float n, float a, float b, float c, float d )
		{
			return ( n - a ) / ( b - a ) * ( d - c ) + c;
		}

		/// <summary>
		/// Truncate the specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		public static int Truncate( float n )
		{
			return ~~(int)n;
		}

		/// <summary>
		/// Rounds the specified value down.
		/// </summary>
		/// <param name="value">Value.</param>
		public static int Floor( float n )
		{
			return (int)n | 0;
		}

		/// <summary>
		/// Round the specified value up.
		/// </summary>
		/// <param name="value">Value.</param>
		public static int Ceil( float n )
		{
			return (int)n + ( (int)n < 0 ? 0 : 1 ) >> 0;
		}

		/// <summary>
		/// Swaps int a and b, without any allocation.
		/// </summary>
		/// <param name="a">Integer A.</param>
		/// <param name="b">Integer B.</param>
		public static void SwapNonAlloc( ref int a, ref int b )
		{
			a = a ^ b;
			b = b ^ a;
			a = a ^ b;
		}
	}
}