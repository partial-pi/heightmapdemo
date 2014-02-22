// --------------------------------------------------------------------------------------------- //
// (c) partial pi 2012-2014
// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// http://creativecommons.org/licenses/by-nc/4.0/deed.en_US
// --------------------------------------------------------------------------------------------- //

using UnityEngine;
using System.Collections;

namespace pi.unity.heightmapdemo
{
	/// <summary>
	/// Teleports game object entering this trigger and resets the mask if desired.
	/// The demo uses this for when the humanoid hits the water, teleporting the humanoid
	/// back to the teleport position.
	/// </summary>
	public class TeleportGameObject : MonoBehaviour
	{
		// Location to which the humanoid will be transported
		public Vector3 	teleportDestination;

		// mask applied to the teleported object
		public int 		mask;

		// flag indicating whether or not the layermask should be applied
		// to teleported objects
		public bool		applyLayerMask;

		// callback from the engine indicating an object has entered
		// the trigger zone.
		void OnTriggerEnter2D(Collider2D collision )
		{
			collision.transform.position = teleportDestination;

			if ( applyLayerMask )
			{
				collision.gameObject.layer = mask;
			}
		}
	}
}