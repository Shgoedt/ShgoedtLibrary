using UnityEngine;
using System.Collections;
using Shgoedt.ErrorEngine;

/// <summary>
/// ErrorEngineDemo class.
/// </summary>
public class ErrorEngineDemo : MonoBehaviour
{

	public GameObject myGameObject;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		if( !myGameObject )
			Error.ThrowError( ErrorCode.UnassignedVariable, ErrorPriority.Error );
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		myGameObject.name = "";
	}
}
