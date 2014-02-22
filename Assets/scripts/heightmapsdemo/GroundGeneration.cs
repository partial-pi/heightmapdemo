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
	/// Behaviour wrapper class around a 2d heightmap. Allows
	/// for in editor updates to the heightmap
	/// </summary>
	[ExecuteInEditMode]
	public class GroundGeneration : MonoBehaviour 
	{
		/// <summary>
		/// underlying class that does the actual heightmap building work
		/// </summary>
		public  BasicHeightMap2D heightMap     = new BasicHeightMap2D(); 

		/// <summary>
		/// If set to true builds a collider on top of the heightmap.
		/// </summary>
		public  bool 			 buildCollider = true;

		/// <summary>
		/// Check to see if a heightmap has been build. This flag is used
		/// because calls may be made out of order.
		/// </summary>
		private bool _hasBuildHeightMap = false;		

		/// <summary>
		/// Hash used to see if the heightmap has changed
		/// </summary>
		private int _heightMapHash = -1;

		void Start () 
		{
			if ( !_hasBuildHeightMap )
			{
				BuildHeightMap();
			}
		}

		/// <summary>
		/// Builds the height map if it's not up to date. If 
		/// build collider is set to true it will also build an 
		/// edge collider
		/// </summary>
		public void BuildHeightMap()
		{
			MeshFilter filter = GetComponent<MeshFilter>();

			if ( !_hasBuildHeightMap && filter != null)
			{
				heightMap.X = gameObject.transform.position.x;
				heightMap.BuildHeightMap( filter );
				_hasBuildHeightMap = true;
				_heightMapHash =  heightMap.heights.GetHashCode();

				if ( buildCollider )
				{
					EdgeCollider2D topCollider = GetComponent<EdgeCollider2D>();
					
					if ( topCollider == null )
					{
						topCollider = gameObject.AddComponent<EdgeCollider2D>();
					}
					
					topCollider.points = heightMap.heights;
				}
			}
		}

		/// <summary>
		/// Returns the height of the heightmap at the given X position.
		/// Note that the return value only makes sense if the requested value
		/// is within the range.
		/// </summary>
		public float GetHeight( float x )
		{
			return heightMap.GetHeight( x );
		}

		/// <summary>
		/// Returns the normal of the heightmap at the given X position.
		/// Note that the return value only makes sense if the requested value
		/// is within the range.
		/// </summary>
		public Vector2 GetNormal( float x )
		{
			return heightMap.GetNormal( x );
		}

		/// <summary>
		/// Update this instance in the editor if the data is out of sync.
		/// </summary>
		public void Update()
		{
			if ( Application.isEditor )
			{
				if ( _heightMapHash !=  heightMap.heights.GetHashCode() )
				{
					_hasBuildHeightMap = false;
					BuildHeightMap();
				}
			}
		}
	}
}