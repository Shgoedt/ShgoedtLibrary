using UnityEngine;
using System.Collections;

namespace Shgoedt.ErrorEngine
{

	public enum ErrorCode
	{
		NullError = 0,
		UnassignedVariable = 1,
		MissingComponent = 2,
	}

	public enum ErrorPriority
	{
		Log = 1,
		Warning = 2,
		Error = 4
	}

	/// <summary>
	/// ErrorEngine class.
	/// </summary>
	public sealed class Error
	{
		/// <summary>
		/// Throws the error.
		/// </summary>
		/// <param name="error">Error.</param>
		public static void ThrowError( ErrorCode error ) { ThrowError( error, ErrorPriority.Error ); }
		/// <summary>
		/// Throws the error.
		/// </summary>
		/// <param name="error">Error.</param>
		/// <param name="priority">Priority.</param>
		public static void ThrowError( ErrorCode error, ErrorPriority priority )
		{
			// Create a string to display as error message.
			string message = string.Empty;

			// Append text based on ErrorCode.
			switch( error )
			{
#if UNITY_EDITOR
			case ErrorCode.NullError:
				message = "ErrorCode.NullError should only be used for testing!";
				break;
#endif
			case ErrorCode.UnassignedVariable:
				message = "Unassigned Variable: A variable has not been assigned.";
				break;
			case ErrorCode.MissingComponent:
				message = "Missing Component: A component you are trying to access is not present.";
				break;
			default:
				message = "ErrorCode Error: No ErrorCode specified.";
				break;
			}

			// Throw the appropriate error based on ErrorPriority.
			switch( (int)priority )
			{
			case 1:
				Debug.Log( message );
				break;
			case 2:
				Debug.LogWarning( message );
				break;
			case 4:
				Debug.LogError( message );
				break;
			default:
				message += "(No ErrorPriority specified!)";
				Debug.Log( message );
				break;
			}
		}
	}
}
