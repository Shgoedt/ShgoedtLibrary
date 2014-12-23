#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Shgoedt
{
	/// <summary>
	/// PreviewSettingsEditor class.
	/// </summary>
	public class PreviewSettingsEditor : EditorWindow
	{
		/// <summary>
		/// Shows the window.
		/// Add a button in 'Edit/Preview Settings'.
		/// </summary>
		[MenuItem( "Edit/Preview Settings" )]
		static void ShowWindow()
		{
			// Load the PreviewSettings.
			PreviewSettings.LoadAsset();

			// Get existing open window or if none, make a new one:
			PreviewSettingsEditor window = (PreviewSettingsEditor)EditorWindow.GetWindow( typeof( PreviewSettingsEditor ) );
			// Focus on the window.
			window.Focus();
		}

		/// <summary>
		/// Raised after this Window is destroyed.
		/// </summary>
		void OnDestroy()
		{
			PreviewSettings.SaveAsset();
		}

		/// <summary>
		/// Raised after OnGUI is called.
		/// </summary>
		void OnGUI()
		{
			// Draw a label for the Preview Settings
			GUILayout.Label( "Preview Settings", EditorStyles.boldLabel );

			EditorGUI.BeginChangeCheck();

			PreviewSettings.audio = EditorTools.DrawHeader( "Audio Channel" + ChannelStatus( PreviewSettings.audio ), "ps_audio" );
			if( PreviewSettings.audio )
			{
				EditorTools.BeginContents();

				PreviewSettings.audioVolume = EditorGUILayout.Slider( "Audio Volume", PreviewSettings.audioVolume, 0f, 1f );

				EditorTools.EndContents();
			}

			PreviewSettings.camera = EditorTools.DrawHeader( "Camera Channel" + ChannelStatus( PreviewSettings.camera ), "ps_camera" );
			if( PreviewSettings.camera )
			{
				EditorTools.BeginContents();

				PreviewSettings.cameraFoV = EditorGUILayout.Slider( "Field of View", PreviewSettings.cameraFoV, 1f, 179f );

				EditorGUILayout.PrefixLabel("Camera Clip Planes");
				EditorGUILayout.MinMaxSlider( ref PreviewSettings.cameraNearPlane, ref PreviewSettings.cameraFarPlane, 0f, 1000f );

				EditorGUILayout.BeginVertical();
				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.PrefixLabel( "Near" );
				PreviewSettings.cameraNearPlane = EditorGUILayout.FloatField( PreviewSettings.cameraNearPlane );
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				
				EditorGUILayout.PrefixLabel( "Far" );
				PreviewSettings.cameraFarPlane = EditorGUILayout.FloatField( PreviewSettings.cameraFarPlane );
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				
				EditorTools.EndContents();
			}

			if( EditorGUI.EndChangeCheck() )
			{
				PreviewSettings.SaveAsset();
			}
		}

		/// <summary>
		/// Gets the channel status as a string.
		/// </summary>
		/// <returns>The status.</returns>
		/// <param name="state">If set to <c>true</c> state.</param>
		private string ChannelStatus( bool state )
		{
			return ( state == true ? " (Enabled)" : " (Disabled)" );
		}

		/// <summary>
		/// Loads the <see cref="UnityEngine.Camera"/> settings from the PreviewSettings to the selected <see cref="UnityEngine.Camera"/>.
		/// </summary>
		[MenuItem("CONTEXT/Camera/Load from PreviewSettings")]
		static void LoadCameraSettings()
		{
			PreviewSettings.SetCamera( Selection.activeGameObject.GetComponent<Camera>() );
		}

		/// <summary>
		/// Loads the <see cref="UnityEngine.AudioSource"/> settings from the PreviewSettings to the selected <see cref="UnityEngine.AudioSource"/>.
		/// </summary>
		[MenuItem("CONTEXT/AudioSource/Load from PreviewSettings")]
		static void LoadAudioSettings()
		{
			PreviewSettings.SetAudioSource( Selection.activeGameObject.GetComponent<AudioSource>() );
		}
	}
}
#endif