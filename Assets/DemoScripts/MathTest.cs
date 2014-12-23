using UnityEngine;
using System.Collections;
using Shgoedt;

/// <summary>
/// MathTest class.
/// </summary>
public class MathTest : MonoBehaviour
{

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		print ( Math.Map( 1f, 0f, 2f, 100f, 200f ) ); // 150
		print ( Math.Truncate( -2.45f ) ); // -2
		print ( Math.Truncate( 3.45f ) ); // 3
		print ( Math.Floor( -7.85f ) ); // -8
		print ( Math.Ceil( 45.14f ) ); // 46

		int a = 1;
		int b = 2;

		print ( string.Format( "A == {0}, B == {1}", a, b ) );
		Math.SwapNonAlloc( ref a, ref b );
		print ( string.Format( "A == {0}, B == {1}", a, b ) );
	}
}
