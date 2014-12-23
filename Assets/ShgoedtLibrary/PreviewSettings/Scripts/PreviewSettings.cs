#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.Callbacks;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Shgoedt
{
	/// <summary>
	/// PreviewSettings class.
	/// </summary>
	public sealed class PreviewSettings
	{

		#region Constants
		
		/// <summary>
		/// The size of the data.
		/// </summary>
		const int dataSize = 64;
		
		/// <summary>
		/// Gets the data path.
		/// </summary>
		/// <value>The data path.</value>
		static string dataPath
		{
			get
			{
				return ( Application.dataPath ).Replace( "/Assets", "/ProjectSettings/PreviewSettings.asset" );
			}
		}

		#endregion


		#region Audio Properties

		/// <summary>
		/// Wether to use audio settings or not.
		/// </summary>
		public static bool audio;

		/// <summary>
		/// The audio volume.
		/// </summary>
		public static float audioVolume;

		#endregion


		#region Camera Properties

		/// <summary>
		/// Wether to use camera settings or not.
		/// </summary>
		public static bool camera;

		/// <summary>
		/// The camera field of view.
		/// </summary>
		public static float cameraFoV;

		/// <summary>
		/// The camera near clip plane.
		/// </summary>
		public static float cameraNearPlane;

		/// <summary>
		/// The camera far clip plane.
		/// </summary>
		public static float cameraFarPlane;

		#endregion


		#region Channel 3 Properties



		#endregion


		#region PostProcess Method

		/// <summary>
		/// Called after the scene is processed.
		/// </summary>
		[PostProcessScene]
		public static void OnPostprocessScene()
		{
			// Load the data from the PreviewSettings.
			LoadAsset();

			// Set the timeScale to 0.
			Time.timeScale = 0f;

			// If we enabled audio settings, set them.
			if( PreviewSettings.audio )
				SetAudioSettings();

			// If we enabled camera settings, set them.
			if( PreviewSettings.camera )
				SetCameraSettings();

			// Set the timeScale back.
			Time.timeScale = 1f;
		}

		#endregion


		#region Private Methods

		/// <summary>
		/// Sets the audio settings.
		/// </summary>
		private static void SetAudioSettings()
		{
			// Load all audio sources.
			AudioSource[] audioSources = Object.FindObjectsOfType<AudioSource>();
			for( int i = 0; i < audioSources.Length; ++i )
			{
				// Set the volume.
				audioSources[i].volume = audioVolume;
			}
		}

		/// <summary>
		/// Sets the camera settings.
		/// </summary>
		private static void SetCameraSettings()
		{
			// Load all audio sources.
			Camera[] cameras = Object.FindObjectsOfType<Camera>();
			for( int i = 0; i < cameras.Length; ++i )
			{
				// Set the Camera settings.
				cameras[i].fieldOfView = cameraFoV;
				cameras[i].nearClipPlane = cameraNearPlane;
				cameras[i].farClipPlane = cameraFarPlane;
			}
		}

		#endregion

		
		#region Public Methods

		/// <summary>
		/// Sets a single <see cref="UnityEngine.AudioSource"/>.
		/// </summary>
		/// <param name="audioSource">Audio source.</param>
		public static void SetAudioSource( AudioSource audioSource )
		{
			// Load the PreviewSettings.
			LoadAsset();
			// Set the AudioSource settings.
			audioSource.volume = audioVolume;
		}

		/// <summary>
		/// Sets a single <see cref="UnityEngine.Camera"/>.
		/// </summary>
		/// <param name="audioSource">Audio source.</param>
		public static void SetCamera( Camera camera )
		{
			// Load the PreviewSettings.
			LoadAsset();
			// Set the Camera settings.
			camera.fieldOfView = cameraFoV;
			camera.nearClipPlane = cameraNearPlane;
			camera.farClipPlane = cameraFarPlane;
		}

		/// <summary>
		/// Saves the PreviewSettings asset.
		/// </summary>
		public static void SaveAsset()
		{
			// Make a new object[].
			object[] data = GetData();
			
			// Construct a new FileStream.
			FileStream fs = File.Create( dataPath );
			
			// Construct a BinaryFormatter and use it to serialize the data to the stream.
			BinaryFormatter formatter = new BinaryFormatter();
			try
			{
				// Serialize the contents in the filestream.
				formatter.Serialize( fs, data );
			}
			catch( SerializationException e ) { Debug.LogWarning( "Failed to serialize. Reason: " + e.Message ); }
			finally { fs.Close(); }
		}

		/// <summary>
		/// Loads the PreviewSettings asset.
		/// </summary>
		public static void LoadAsset()
		{
			// If the file doesn't exist, return.
			if( !File.Exists( dataPath ) ) return;

			// Make a new object[].
			object[] data = new object[dataSize];
			
			// Open the file containing the data that you want to deserialize.
			FileStream fs = new FileStream( dataPath, FileMode.Open );
			try 
			{
				// Construct a BinaryFormatter and use it to serialize the data to the stream.
				BinaryFormatter formatter = new BinaryFormatter();
				
				// Deserialize the contents from the file.
				data = (object[])formatter.Deserialize( fs );
			}
			catch( SerializationException e ) { Debug.LogError( "Failed to deserialize. Reason: " + e.Message ); }
			finally { fs.Close(); }
			
			SetData( data );
		}

		#endregion


		#region Data Methods

		/// <summary>
		/// Gets the data.
		/// </summary>
		/// <returns>The data.</returns>
		private static object[] GetData()
		{
			object[] output = new object[dataSize];

			// Audio
			output[0] = PreviewSettings.audio;
			output[1] = PreviewSettings.audioVolume;
			output[2] = 0;
			output[3] = 0;
			output[4] = 0;
			output[5] = 0;
			output[6] = 0;
			output[7] = 0;
			output[8] = 0;
			output[9] = 0;

			// Camera
			output[10] = PreviewSettings.camera;
			output[11] = PreviewSettings.cameraFoV;
			output[12] = PreviewSettings.cameraNearPlane;
			output[13] = PreviewSettings.cameraFarPlane;
			output[14] = 0;
			output[15] = 0;
			output[16] = 0;
			output[17] = 0;
			output[18] = 0;
			output[19] = 0;
			output[20] = 0;

			// Channel 3
			output[21] = 0;
			output[22] = 0;
			output[23] = 0;
			output[24] = 0;
			output[25] = 0;
			output[26] = 0;
			output[27] = 0;
			output[28] = 0;
			output[28] = 0;
			output[28] = 0;
			output[30] = 0;

			return output;
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		private static void SetData( object[] data )
		{
			// Audio 0-9.
			PreviewSettings.audio = (bool)data[0];
			PreviewSettings.audioVolume = (float)data[1];

			// Camera 10-19.
			PreviewSettings.camera = (bool)data[10];
			PreviewSettings.cameraFoV = (float)data[11];
			PreviewSettings.cameraNearPlane = (float)data[12];
			PreviewSettings.cameraFarPlane = (float)data[13];
		}

		#endregion
	}
}
#endif