// --------------------------------------------------------------------------------------------- //
// (c) partial pi 2012-2014
// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// http://creativecommons.org/licenses/by-nc/4.0/deed.en_US
// --------------------------------------------------------------------------------------------- //

using UnityEngine;

/// <summary>
/// 2D Heightmap implementation, creates a heightmap mesh from a bunch of
/// vector2s
/// </summary>
[System.Serializable]
public class BasicHeightMap2D : IHeightMap2D
{
	// heights that make up the map
	public  Vector2[] heights;	

	// area over which the heightmap will be spread
	public  float	  width;

	// mesh represeting the heightmap
	public Mesh	HeightMapMesh
	{
		get;
		private set;
	}

	// x position of the heightmap
	public float X
	{
		get;
		set;
	}

	// Width of the heigth map. 
	public float Width
	{
		get
		{
			return width;
		}

		set
		{
			width = value;
		}
	}

	// heights that make up the map
	public Vector2[] Heights
	{
		get
		{
			return heights;
		}
		
		set
		{
			heights = value;
		}
	}

	// build the heightmap using the given filter and the values of the heights property
	public void BuildHeightMap( MeshFilter filter )
	{
		BuildHeightMap( filter, null, 0 );
	}

	// builds a heightmap using the given filter 
	// optionally uses the baselayer and offset to build on top of the values of the heights propert 
	public void BuildHeightMap( MeshFilter filter, Vector2[] baseLayer, float offsetFromBaseLayer )
	{
		// if no heights are set bail out - may be that the user hasn't intialized the data
		if ( heights == null || heights.Length == 0 ) return;

		Vector3[] vertices = new Vector3[ heights.Length * 2 ];
		Vector3[] normals  = new Vector3[ heights.Length * 2 ];
		Vector2[] uvs	   = new Vector2[ heights.Length * 2 ];
		int[] 	  polygons = new int[ ( heights.Length - 1 ) * 2 * 3 ];
		float     widthPerSection = width / ( heights.Length - 1 );

		if ( HeightMapMesh != null )
		{
			HeightMapMesh.Clear();
		}
		else
		{
			HeightMapMesh = new Mesh();
		}

		filter.mesh = HeightMapMesh;
		
		for ( int i = 0; i < heights.Length; ++i )
		{
			float bottom = 0;

			if ( heights[ i ].x == 0 )
			{
				heights[ i ].x = i * widthPerSection;
			}

			if ( baseLayer != null )
			{
				bottom = baseLayer[ i ].y + offsetFromBaseLayer;
			}

			vertices[ i * 2 ] = new Vector3( heights[ i ].x, bottom, 0 );
			vertices[ i * 2 + 1 ] = new Vector3( heights[ i ].x, heights[ i ].y, 0 );
			
			normals[ i * 2 ] = Vector3.forward; 
			normals[ i * 2 + 1 ] = Vector3.forward;
		}
		
		for ( int i = 0; i < heights.Length - 1; ++i )
		{
			polygons[ i * 6 + 0 ] = i * 2 + 0;
			polygons[ i * 6 + 1 ] = i * 2 + 1;
			polygons[ i * 6 + 2 ] = i * 2 + 2;
			
			polygons[ i * 6 + 3 ] = i * 2 + 2;
			polygons[ i * 6 + 4 ] = i * 2 + 1;
			polygons[ i * 6 + 5 ] = i * 2 + 3;
		}
		
		for ( int i = 0; i < uvs.Length; i += 2 )
		{
			if ( ( i % 4 ) == 0 )
			{
				uvs[ i + 0 ] = new Vector2( 0, 0 );
				uvs[ i + 1 ] = new Vector2( 0, 1 );
			}
			else
			{
				uvs[ i + 0 ] = new Vector2( 1, 0 );
				uvs[ i + 1 ] = new Vector2( 1, 1 );
			}
		}
		
		HeightMapMesh.vertices  = vertices;
		HeightMapMesh.normals   = normals;
		HeightMapMesh.uv        = uvs;
		HeightMapMesh.triangles = polygons;
	}
	// checks if the given value is >= X and smaller than X + Width
	public bool  IsInRange( float x )
	{
		return ( x >= X && x < X + Width );
	}

	// returns the heigth of this heightmap at the given position
	public float GetHeight( float worldXPosition )
	{
		if ( IsInRange( worldXPosition ) )
		{
			float widthPerSection = width / ( heights.Length - 1 );
			float testPosition    = worldXPosition - X;
			int   minHeightIndex  = (int) ( testPosition / widthPerSection );
			
			if ( minHeightIndex < heights.Length  - 1)
			{
				float factor 	 = ( testPosition - ( minHeightIndex * widthPerSection ) ) / widthPerSection; 
				return heights[ minHeightIndex ].y 
					+ ( heights[ minHeightIndex  + 1].y - heights[ minHeightIndex ].y ) * factor; 
			}
			
			return heights[ heights.Length - 1 ].y;
		}
		
		return -1;
	}

	// returns the normal at the given position
	public Vector2 GetNormal( float worldXPosition )
	{
		if ( IsInRange( worldXPosition ) )
		{
			float widthPerSection = width / ( heights.Length - 1 );
			float testPosition    = worldXPosition - X;
			int   minHeightIndex  = (int) ( testPosition / widthPerSection );
			
			if ( minHeightIndex < heights.Length  - 1)
			{
				Vector2 normal = new Vector2( widthPerSection, heights[ minHeightIndex + 1 ].y - heights[ minHeightIndex ].y );

				normal.Normalize();
				return new Vector2( -normal.y, normal.x );
			}
		}
		
		return new Vector2( 0, 1 );
	}
}