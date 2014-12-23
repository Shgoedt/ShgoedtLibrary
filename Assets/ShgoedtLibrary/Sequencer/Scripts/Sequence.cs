using UnityEngine;
using System.Collections;

namespace Shgoedt.Sequences
{
	/// <summary>
	/// <see cref="Shgoedt.Sequences.Sequence"/> class.
	/// Returns a position over a line of points.
	/// </summary>
	public class Sequence : MonoBehaviour
	{

		#region Properties

		/// <summary>
		/// The current position on the <see cref="Shgoedt.Sequences.Sequence"/>.
		/// </summary>
		public Vector3 position;

		/// <summary>
		/// The speed.
		/// </summary>
		public float speed;

		/// <summary>
		/// The <see cref="UnityEngine.Vector3"/> array of points.
		/// </summary>
		public Vector3[] points;

		/// <summary>
		/// Should it play on Awake()?
		/// </summary>
		public bool playOnAwake;

		Vector3 basePosition = Vector3.zero;

		/// <summary>
		/// The is playing.
		/// </summary>
		bool isPlaying = false;

		/// <summary>
		/// The index number of the current point.
		/// </summary>
		int index = 0;

		/// <summary>
		/// The index number of the next point.
		/// </summary>
		int next = 1;

		/// <summary>
		/// The timer.
		/// </summary>
		float timer = 0f;

		/// <summary>
		/// The distance between 'points[index]' and 'points[next]'.
		/// </summary>
		float distance = 0f;

		#endregion


		#region Built-in methods

		/// <summary>
		/// Reset this <see cref="Shgoedt.Sequences.Sequence"/>.
		/// </summary>
		void Reset()
		{
			// Set the base position.
			basePosition = transform.position;
			// Assign 3 new points.
			points = new Vector3[3] { basePosition + Vector3.left, basePosition + Vector3.zero, basePosition + Vector3.right };
			// Set 'index' to 0.
			index = 0;
			// Set 'next' index to 1.
			next = 1;
			// Set 'timer' to 0f.
			timer = 0f;
		}

		/// <summary>
		/// Awake this <see cref="Shgoedt.Sequences.Sequence"/>.
		/// </summary>
		void Awake()
		{
			// If we should play on 'Awake()'...
			if( playOnAwake )
				// ... set 'isPlaying' to 'true'.
				isPlaying = true;
		}
		
		/// <summary>
		/// Update this <see cref="Shgoedt.Sequences.Sequence"/>.
		/// </summary>
		void Update()
		{
			// If the Sequence is not playing, abort the method.
			if( !isPlaying ) return;

			// Increment 'timer' by 'Time.deltaTime'.
			timer += Time.deltaTime;

			// Calculate the position.
			position = Vector3.Lerp( points[index], points[next], Math.Map( timer * speed, 0f, distance, 0f, 1f ) );

			// If the position
			if( position == points[next] )
				// Prepare for the next point.
				SetPoint( next );
		}

		#endregion


		#region Public methods

		/// <summary>
		/// Play this <see cref="Shgoedt.Sequences.Sequence"/>.
		/// </summary>
		public void Play()
		{
			// Set 'isPlaying' to true.
			isPlaying = true;
		}

		/// <summary>
		/// Stop this <see cref="Shgoedt.Sequences.Sequence"/>.
		/// </summary>
		public void Stop()
		{
			// Set 'isPlaying' to false.
			isPlaying = false;
		}

		#endregion


		#region Public methods

		/// <summary>
		/// Sets the point.
		/// </summary>
		/// <param name="index">Index.</param>
		private void SetPoint( int i )
		{
			// Increment 'index'
			index = i;
			// Repeat 'index' between 0 and 'points.Length'.
			index = index % points.Length;

			// Set 'next'.
			next = index + 1;
			// Clamp 'next' between 0 and 'points.Length'.
			next = next % points.Length;

			// Set 'timer' to 0f.
			timer = 0f;

			// Set 'distance'.
			distance = ( points[index] - points[next] ).magnitude;
		}

		#endregion


		#region Static methods

		/// <summary>
		/// Gets the <see cref="Shgoedt.Sequences.Sequence"/> by name.
		/// </summary>
		/// <param name="name">Sequence name.</param>
		public static Sequence GetSequence( string name )
		{
			// Get the GameObject by name.
			GameObject go = GameObject.Find( name );

			// If no GameObject was found by that name, abort the method.
			if( go == null )
			{
				Debug.LogWarning( string.Format( "Sequence: No Sequence with name {0} was found!", name ) );
				return null;
			}

			// Get the Sequence from the GameObject.
			Sequence sequence = go.GetComponent<Sequence>();

			// If no GameObject was found by that name, abort the method.
			if( sequence == null )
			{
				Debug.LogWarning( string.Format( "Sequence: GameObject {0} was found, but no Sequence component was attached!", name ) );
				return null;
			}

			// Return the sequence.
			return sequence;
		}

		#endregion
	}
}