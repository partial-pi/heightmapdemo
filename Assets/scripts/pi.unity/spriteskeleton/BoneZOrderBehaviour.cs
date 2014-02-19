// --------------------------------------------------------------------------------------------- //
// (c) partial pi 2012-2014
// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// http://creativecommons.org/licenses/by-nc/4.0/deed.en_US
// --------------------------------------------------------------------------------------------- //

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using pi.unity.util;

namespace pi.unity.spriteskeleton
{
	/// <summary>
	///   Sprite skeletons in the perspective used in the demo come with a nasty
	/// side effect that I haven't been able to solve through something less
	/// manual than this. Maybe there is a simpler solution, but I haven't found 
	/// that yet. 
	///   In the meantime this class takes care of z order fighting when 
	/// dealing with composite sprites by updating the z order based on
	/// the bottom yPosition of the composite root.
	/// 
	/// </summary>
	public class BoneZOrderBehaviour : MonoBehaviour
	{
	
		/// <summary>
		/// Cached original sorting orders, filled at the start. Note
		/// that the assumption is that bones are ordered as intended at the start.
		/// </summary>
		public BoneZOrderCollection bonesZOrder = new BoneZOrderCollection();

		/// <summary>
		/// Offset that is added to the transform.position used determine a baseline.
		/// This value determines the draworder, the lower the gameobject is 
		/// on the screen the higher the draw order is. 
		/// </summary>
		public float			zOrderLowerBound;

		/// <summary>
		/// Resolved / cached renderers. The renderers will be cached at the start so the 
		/// renderers only need to be found once.
		/// </summary>
		private List<SpriteRenderer> _boneRenderers;

		/// <summary>
		/// Finds all spriterenderes and prepares the root sorting orders.
		/// </summary>
		void Start () 
		{
			_boneRenderers = gameObject.FindAllComponentsByType<SpriteRenderer>();
			bonesZOrder.Initialize( _boneRenderers );
		}
		
		/// <summary>
		/// updates the z order of all child objects if the root gameobject has moved
		/// </summary>
		void Update () 
		{
			float lowerBound = ( zOrderLowerBound * transform.lossyScale.y )+ transform.position.y;
			bonesZOrder.Update( _boneRenderers, lowerBound );		
		}

		/// <summary>
		/// Callback from the editor indicating the behaviour can draw 
		/// a gizmo as the gameobject is selected.
		/// </summary>
		public void OnDrawGizmosSelected()
		{
			BoneZOrderCollection.DrawGizmo( zOrderLowerBound, transform );
		}
	}
}