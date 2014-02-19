using UnityEngine;
using System.Collections;

namespace pi.unity.heightmapdemo
{
	[ExecuteInEditMode]
	public class GroundLayer : MonoBehaviour 
	{
		public GameObject GroundGenerationObject;

		private BasicHeightMap2D heightMap = new BasicHeightMap2D(); 
		public 	float height = 1;
		public  float offset = 0;
		public  bool buildCollider = true;


		private bool _hasBuildHeightMap = false;		
		private int _heightMapHash;
		private int _targetHeightMapHash;

		void Start () 
		{
			if ( !_hasBuildHeightMap )
			{
				GroundGeneration g = GroundGenerationObject.GetComponent<GroundGeneration>();
				g.BuildHeightMap();
				BuildHeightMap( g.heightMap, height );
			}
		}
		
		public void BuildHeightMap( IHeightMap2D source, float height )
		{
			heightMap.X = source.X;
			heightMap.Width = source.Width;

			int len = source.Heights.Length;

			heightMap.heights = new Vector2[len];

			for ( int i = 0; i < len; ++i )
			{
				heightMap.heights[ i ] = new Vector2( source.Heights[ i ].x, source.Heights[ i ].y + height ); 
			}

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
			_hasBuildHeightMap = true;
			_heightMapHash = heightMap.heights.GetHashCode();
			_targetHeightMapHash = source.Heights.GetHashCode();
		}


		public void Update()
		{
			if ( Application.isEditor && GroundGenerationObject != null  )
			{
				GroundGeneration g = GroundGenerationObject.GetComponent<GroundGeneration>();

				if ( g != null && ( _heightMapHash !=  heightMap.heights.GetHashCode()
				                   || _targetHeightMapHash != g.heightMap.heights.GetHashCode() ) )
				{
					_hasBuildHeightMap = false;
					g.BuildHeightMap();
					BuildHeightMap( g.heightMap, height );
				}
			}
		}

		public float GetHeight( float x )
		{
			return heightMap.GetHeight( x );
		}
	}
}