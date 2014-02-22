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
	/// Ground layer puts a layer on top of a Ground Generation object.
	/// In the demo this creates the grass toplayer on top of the main land
	/// and island
	/// </summary>
	[ExecuteInEditMode]
	public class GroundLayer : MonoBehaviour 
	{
		// object that serves as the underlying ground
		public GameObject GroundGenerationObject;

		// height of the layer
		public 	float height = 1;

		// position y offset
		public  float offset = 0;

		// if set will build an edge collider
		public  bool buildCollider = true;

		// heightmap object that actually builds the data
		private BasicHeightMap2D heightMap = new BasicHeightMap2D(); 

		private bool _hasBuildHeightMap = false;		

		// hash of this heightmap - is used to check if an update to the heigthmap is needed
		private int _heightMapHash;

		// has of the object that creates the lower ground
		// - is used to check if an update to the heigthmap is needed
		private int _groundGenerationObjectHash;

		// builds a heightmap if none has been build
		void Start () 
		{
			if ( !_hasBuildHeightMap )
			{
				GroundGeneration g = GroundGenerationObject.GetComponent<GroundGeneration>();
				g.BuildHeightMap();
				BuildHeightMap( g.heightMap, height );
			}
		}

		// build a layer on top of the given source
		public void BuildHeightMap( IHeightMap2D source, float deltaHeight )
		{
			// copy the source x pos and width
			heightMap.X = source.X;
			heightMap.Width = source.Width;

			int len = source.Heights.Length;

			// copy the heights adding the deltaheight to create a difference
			heightMap.heights = new Vector2[len];

			for ( int i = 0; i < len; ++i )
			{
				heightMap.heights[ i ] = new Vector2( source.Heights[ i ].x, source.Heights[ i ].y + deltaHeight ); 
			}

			// build a physics collider stopping any rigidbody trying to pass it. Unless it's not...
			// For whatever reason. Then it falls straight through.
			if ( buildCollider )
			{
				EdgeCollider2D topCollider = GetComponent<EdgeCollider2D>();

				if ( topCollider == null )
				{
					topCollider = gameObject.AddComponent<EdgeCollider2D>();
				}

				topCollider.points = heightMap.heights;
			}

			heightMap.BuildHeightMap( GetComponent<MeshFilter>(), source.Heights, offset );

			// mark that a heightmap has been made
			_hasBuildHeightMap = true;
			_heightMapHash = heightMap.heights.GetHashCode();
			_groundGenerationObjectHash = source.Heights.GetHashCode();
		}

		// this update checks if the input for the heightmap has changed
		// and it needs recreating
		public void Update()
		{
			if ( Application.isEditor && GroundGenerationObject != null  )
			{
				GroundGeneration g = GroundGenerationObject.GetComponent<GroundGeneration>();

				if ( g != null && ( _heightMapHash !=  heightMap.heights.GetHashCode()
				                   || _groundGenerationObjectHash != g.heightMap.heights.GetHashCode() ) )
				{
					_hasBuildHeightMap = false;
					g.BuildHeightMap();
					BuildHeightMap( g.heightMap, height );
				}
			}
		}

		// returns the height of the layer at the given position
		public float GetHeight( float x )
		{
			return heightMap.GetHeight( x );
		}
	}
}