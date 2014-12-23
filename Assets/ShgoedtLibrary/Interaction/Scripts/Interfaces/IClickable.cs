using UnityEngine;
using System.Collections;


namespace Shgoedt.Interaction
{
	/// <summary>
	/// The state of the <see cref="Shgoedt.Interaction.IClickable"/> object.
	/// </summary>
	public enum IClickableState
	{
		Disabled,
		Normal,
		Hover,
		Pressed
	};
	
	/// <summary>
	/// <see cref="Shgoedt.Interaction.IClickable"/> interface.
	/// Handles clickable interaction objects.
	/// </summary>
	public interface IClickable
	{
		/// <summary>
		/// The state of the <see cref="Shgoedt.Interaction.IClickable"/> object.
		/// </summary>
		IClickableState state { get; set; }
		
		/// <summary>
		/// Gets the position.
		/// </summary>
		/// <value>The position.</value>
		Vector3 Position { get; }
		
		/// <summary>
		/// Determines whether this instance can be used.
		/// </summary>
		/// <returns><c>true</c> if this instance can be used; otherwise, <c>false</c>.</returns>
		bool CanUse();
		
		/// <summary>
		/// Pressed this instance.
		/// </summary>
		void Pressed();
		
		/// <summary>
		/// Released this instance.
		/// </summary>
		void Released();
	}
}