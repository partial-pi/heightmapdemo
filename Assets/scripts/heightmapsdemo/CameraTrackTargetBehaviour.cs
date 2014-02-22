// --------------------------------------------------------------------------------------------- //
// (c) partial pi 2012-2014
// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// http://creativecommons.org/licenses/by-nc/4.0/deed.en_US
// --------------------------------------------------------------------------------------------- //

using UnityEngine;
using System.Collections;

namespace pi.unity.heigthmapdemo
{
	/// <summary>
	/// Allows the camera to stay focussed on a given target
	/// </summary>
	public class CameraTrackTargetBehaviour : MonoBehaviour 
	{
		/// <summary>
		/// The offset applied to the camera after focussing on the target.
		/// </summary>
		public Vector3 offset;

		/// <summary>
		/// The target to focus on.
		/// </summary>
		public GameObject target;

		/// <summary>
		/// Moves the camera to focus on the given target 
		/// </summary>
		void Update () 
		{
			Vector3 pos = target.transform.position;

			pos.z = gameObject.transform.position.z;

			gameObject.transform.position = pos + offset;
		}
	}
}
