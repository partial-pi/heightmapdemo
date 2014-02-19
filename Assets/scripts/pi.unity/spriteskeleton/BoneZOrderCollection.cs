// --------------------------------------------------------------------------------------------- //
// (c) partial pi 2012-2014
// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// http://creativecommons.org/licenses/by-nc/4.0/deed.en_US
// --------------------------------------------------------------------------------------------- //

using System.Collections.Generic;

using UnityEngine;

using pi.unity.util;

namespace pi.unity.spriteskeleton
{
	/// <summary>
	/// In the 3d-ish setup chosen for Smash RPG the draworder of a sprite is having
	/// both an internal component (how does the draworder in bone A relate to bone B 
	/// in this skeleton ? ) but also an external component (how do the draworders of 
	/// the bones of this skeleton A relate to the draw order of the bones in skeleton B? ).
	/// To make sure both work properly this class stores all the internal draworders
	/// and adds them to an externaly determined order. The external factor in this case
    /// comes from an yPosition. ie all skeletons lower on the screen should be drawn after
	/// the skeletons higher on the screen.
	/// 
	/// Note that: A) I'm not sure at all if this is the best approach, a faux 3d setup might
    /// work better, maybe one can manually rig the draw order - not sure.
	/// B) If A does not apply the unity engine will provide something similar already or 
	/// soon in the future ?
	/// </summary>
	[System.Serializable]
	public class BoneZOrderCollection
	{
		/// <summary>
		/// The interval between two y positions. All internal bone draw orders must 
		/// be less than this interval, failing to do so will result in an assert
		/// during initialization.
		/// </summary>
		public int interval = 10; 	  

		/// <summary>
		/// List of the initial sorting orders, capturing the internal sorting order
		/// </summary>
		private int[] _boneSortingOrders;

		/// <summary>
		/// The _last updated Y position of thr root object.
		/// </summary>
		private float _lastUpdateYPosition;

		/// <summary>
		/// Initialize the zOrder collection by caching the sorting orders in the given 
		/// renderes .
		/// </summary>
		/// <param name="renderers">A non null, non empty list of renderers.</param>
		public void Initialize( List<SpriteRenderer> renderers )
		{
#if UNITY_EDITOR
			Assert.NotNull( renderers );
			Assert.IsTrue( renderers.Count > 0, "No renderers provided, can't create a Z-order collection." );
#endif
			_boneSortingOrders = new int[ renderers.Count ];

			// collect the internal z - order of a sprite
			for ( int i = 0; i < _boneSortingOrders.Length; ++i )
			{
				_boneSortingOrders[ i ] = renderers[ i ].sortingOrder;

#if UNITY_EDITOR
				Assert.IsTrue( _boneSortingOrders[ i ] < interval, "Internal sorting of " + renderers[ i ].gameObject.name + " (" + _boneSortingOrders[ i ] + ") "
				              		+ "exceeds the interval ("+ interval + "), this may lead to some z order fighting on sprites close together. "
				              		+ "Set the sorting order to anything less than " + interval
				              ); 
#endif
			}

			_lastUpdateYPosition = float.MinValue;
		}

		/// <summary>
		/// Update the sorting orders of the provided renderers based on the given position of the skeleton
		/// (or its parent gameobject). Note that the provided renderers must match the number
	    ///  of renderers provided during initialization )
		/// </summary>
		public void Update( List<SpriteRenderer> renderers, float position )
		{
#if UNITY_EDITOR
			Assert.NotNull( _boneSortingOrders, "[BoneZOrderCollection] No sorting orders defined, is the class initialized ?" );
			Assert.NotNull( renderers );
			Assert.IsTrue( renderers.Count == _boneSortingOrders.Length, 
			              		"Renderers provided do not match the number of initialized bones." );
#endif
			if ( _lastUpdateYPosition != position && Camera.current != null  )
			{
				Vector3 screenPosition = Camera.current.WorldToScreenPoint( Vector3.up * position );
				
				for ( int i = 0; i < _boneSortingOrders.Length; ++i )
				{
					renderers[ i ].sortingOrder = 
						_boneSortingOrders[ i ] + interval *( Screen.height - ((int)(screenPosition.y)) ); 
				}

				_lastUpdateYPosition = position;
			}
		}

		/// <summary>
		/// Utility method drawing a gizmo displaying the lower bounds on which the 
		/// the sorting order of the bones are based
		/// </summary>
		public static void DrawGizmo( float z, Transform transform )
		{
			float lowerBound = ( z * transform.lossyScale.y ) + transform.position.y;
			
			Gizmos.DrawLine( new Vector3( transform.position.x - 0.5f, lowerBound, transform.position.z )
			                , new Vector3( transform.position.x + 0.5f, lowerBound, transform.position.z ) );
			
			Gizmos.DrawLine( new Vector3( transform.position.x, lowerBound, transform.position.z )
			                , new Vector3( transform.position.x, lowerBound + 0.1f, transform.position.z ) );

		}
	}
}