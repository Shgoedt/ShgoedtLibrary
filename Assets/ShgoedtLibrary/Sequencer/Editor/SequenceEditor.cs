using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Shgoedt.Sequences
{
	/// <summary>
	/// SequenceEditor class.
	/// </summary>
	[CustomEditor( typeof( Sequence ) )]
	public class SequenceEditor : Editor
	{
	
		#region Properties

		/// <summary>
		/// The target <see cref="Shgoedt.Sequences.Sequence"/>.
		/// </summary>
		Sequence sequence;

		#endregion
		
		
		#region Built-in methods

		/// <summary>
		/// Raised on the SceneGUI event.
		/// </summary>
		void OnSceneGUI()
		{
			// Cache the target Sequence.
			sequence = target as Sequence;

			// Make sure there are more then 2 points in the Sequence.
			if( sequence.points.Length < 2 ) return;

			// Draw a Transform handle for each point.
			for( int i = 0; i < sequence.points.Length; ++i )
			{
				EditorGUI.BeginChangeCheck();
				Vector3 pos = Handles.DoPositionHandle( sequence.points[i], Quaternion.identity );
				if( EditorGUI.EndChangeCheck() )
				{
					Undo.RecordObject( target, "MovedSequencePoint" );
					sequence.points[i] = pos;
				}

				Handles.Label( sequence.points[i] + new Vector3( 0f, 0.90f, 0f ), sequence.points[i].ToString() );
				Handles.Label( sequence.points[i] + Vector3.up, i.ToString() );
			}

			EditorUtility.SetDirty(this);
		}
		
		#endregion
		
		
		#region Public methods
		

		
		#endregion
		
		
		#region Static methods
		

		
		#endregion
	}
}
