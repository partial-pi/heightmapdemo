using UnityEngine;
using System.Collections;

namespace pi.unity.heightmapdemo
{
	public class TeleportGameObject : MonoBehaviour {

		public Vector3 	teleportDestination;
		public int 		mask;
		public bool		applyLayerMask;

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