using UnityEngine;
using System.Collections;
using Shgoedt.Interaction;

/// <summary>
/// InputCollider class.
/// </summary>
public class InputCollider2D : MonoBehaviour, IClickable
{

	#region IClickable Implementation

	/// <summary>
	/// Gets the position.
	/// </summary>
	/// <value>The position.</value>
	public Vector3 Position
	{
		get
		{
			return transform.position;
		}
	}

	/// <summary>
	/// The state.
	/// </summary>
	public IClickableState state { get; set; }

	/// <summary>
	/// Determines whether this instance can be used.
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	public bool CanUse()
	{
		return ( collider2D != null && collider2D.enabled );
	}

	/// <summary>
	/// Pressed this instance.
	/// </summary>
	public void Pressed()
	{
		state = IClickableState.Pressed;
	}

	/// <summary>
	/// Released this instance.
	/// </summary>
	public void Released()
	{
		state = IClickableState.Normal;
	}
	
	#endregion

	SpriteRenderer spriteRenderer;

	public Color disabledColor;
	public Color normalColor;
	public Color hoverColor;
	public Color pressedColor;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		spriteRenderer = renderer as SpriteRenderer;
		if( CanUse() )
			state = IClickableState.Normal;
		else
			state = IClickableState.Disabled;
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		switch( state )
		{
		case IClickableState.Disabled:
			spriteRenderer.color = disabledColor;
			break;
		case IClickableState.Normal:
			spriteRenderer.color = normalColor;
			break;
		case IClickableState.Hover:
			spriteRenderer.color = hoverColor;
			break;
		case IClickableState.Pressed:
			spriteRenderer.color = pressedColor;
			break;
		}
	}
}
