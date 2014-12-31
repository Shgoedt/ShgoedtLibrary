using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace Shgoedt
{
	/// <summary>
	/// SelectionGroup class.
	/// </summary>
	public class SelectionGroup : MonoBehaviour
	{

		/// <summary>
		/// The selection group ID.
		/// </summary>
		public int selectionGroupID;

		/// <summary>
		/// If the SelectionGroup should select automatically?
		/// </summary>
		public bool autoSelect;

		/// <summary>
		/// The objects.
		/// </summary>
		public GameObject[] objects;

#if UNITY_EDITOR
		#region Editor Methods
		
		/// <summary>
		/// Selects the objects.
		/// </summary>
		public void SelectObjects()
		{
			// If we are not selecting this object, return.
			if( Selection.activeGameObject != this.gameObject ) return;

			// Assign the new objects.
			GameObject[] targets = new GameObject[objects.Length + 1];
			targets[0] = this.gameObject;
			for( int i = 1; i < objects.Length; ++i )
			{
				targets[i] = objects[i - 1];
			}

			// Set the Selection.objects.
			Selection.objects = targets;
		}
#endregion
#endif
	}
}
