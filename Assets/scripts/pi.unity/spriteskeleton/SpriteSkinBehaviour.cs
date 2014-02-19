// --------------------------------------------------------------------------------------------- //
// (c) partial pi 2012-2014
// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// http://creativecommons.org/licenses/by-nc/4.0/deed.en_US
// --------------------------------------------------------------------------------------------- //

using System;
using System.Collections.Generic;

using UnityEngine;

using pi.unity.util;

namespace pi.unity.spriteskeleton
{
	public class SpriteSkinBehaviour : MonoBehaviour
	{
		/// <summary>
		/// Available skins, the usuage of the skins depends on the application but
		/// one possible use is to treat each skin like a state. Eg 
		/// skin one == walking, idle, skin 2 == climbing, skin 3 == death
		/// and so on.
		/// </summary>
		public SpriteSkeletonSkinAsset[]  	skins;

		/// <summary>
		/// Random seed used by this behaviour, if this value is -1 a random
		/// seed will be randomly chosen at the start. This seed is used
		/// to get a random but consistent permutation from the kin.
		/// </summary>
		public int 			 		randomSeed = -1;

		/// <summary>
		/// Lower bound used to determine the z order (see BoneZOrderCollection).
		/// </summary>
		public float 		 		zOrderLowerBound;

		/// <summary>
		/// Class that maintains the z order of the bones.
		/// </summary>
		public BoneZOrderCollection bonesZOrder = new BoneZOrderCollection();

		/// <summary>
		/// flag indicating whether or not the behaviour should maintain a z order
		/// using the BoneZOrderCollection.
		/// </summary>
		public bool					updateZOrder = true;

		/// <summary>
		/// Cached list of all sprite renderer components in the descendants of this
		/// skeleton.
		/// </summary>
		private List<SpriteRenderer> _boneRenderers;

		/// <summary>
		/// The current selected skin applied to the skeleton. This value holds
		/// the index of the applied skin .
		/// </summary>
		private int				     _currentSkin;

		/// <summary>
		/// Start this instance. Finds all spriterenderes and creates a randomseed
		/// if desired, then applies a random permutation from skin 0.
		/// </summary>
		public void Start()
		{
			_currentSkin  = int.MinValue;
			_boneRenderers = gameObject.FindAllComponentsByType<SpriteRenderer>();

			if ( skins.Length > 0 )
			{
				if ( randomSeed < 0 )
				{
					randomSeed = UnityEngine.Random.Range( 0, int.MaxValue );
				}

				skins[ 0 ].ApplyRandom( _boneRenderers, randomSeed );
			}

			bonesZOrder.Initialize( _boneRenderers );
		}

		/// <summary>
		/// Updates the z order if so desired.
		/// </summary>
		public void Update()
		{
			if ( updateZOrder  )
			{
				float lowerBound = ( zOrderLowerBound * transform.lossyScale.y )+ transform.position.y;
				bonesZOrder.Update( _boneRenderers, lowerBound );
			}
		}

		/// <summary>
		/// Callback from the editor indicating a gizmo can be drawn. This behaviour
		/// will draw the z-order lower bound if this behaviour is set to update its
		/// z order.
		/// </summary>
		public void OnDrawGizmosSelected()
		{
			if ( updateZOrder )
			{
				BoneZOrderCollection.DrawGizmo( zOrderLowerBound, transform );
			}
		}

		/// <summary>
		/// Sets the skin to use. The applied skin is a random permutation
		/// based on this behaviour's random seed.
		/// </summary>
		/// <param name="stateID">State I.</param>
		public void SetSkin( int stateID )
		{
			if ( stateID < skins.Length && skins[ stateID ] != null && _currentSkin != stateID )
			{
				_currentSkin = stateID;
				skins[ stateID ].ApplyRandom( _boneRenderers, randomSeed ); 
			}
		}
	}
}