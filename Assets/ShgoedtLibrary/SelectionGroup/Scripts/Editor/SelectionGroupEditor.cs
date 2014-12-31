#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Shgoedt
{
	/// <summary>
	/// SelectionGroupEditor class.
	/// </summary>
	[CustomEditor( typeof( SelectionGroup ) )]
	public class SelectionGroupEditor : Editor
	{

		SelectionGroup sGroup
		{
			get
			{
			 	return target as SelectionGroup;
			}
		}

		#region Built-in Methods
		
		/// <summary>
		/// Update this instance.
		/// </summary>
		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI();

			if( sGroup == null ) return;
			GUILayout.Space( 3f );
			if( sGroup.autoSelect && sGroup.objects.Length > 0 )
				sGroup.SelectObjects();
			else
				if( GUILayout.Button( "Select") )
					sGroup.SelectObjects();
		}

		#endregion
	}
}

#endif