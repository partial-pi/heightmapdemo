using UnityEngine;
using System.Collections.Generic;
using System;


[Serializable]
public class HeightMap2DCollection 
{
	private List<IHeightMap2D> _heightmaps = new List<IHeightMap2D>();

	public void AddHeightMap( IHeightMap2D heightmap )
	{
		_heightmaps.Add( heightmap );
	}

	public Nullable<Vector2> TestIntersection( Vector2 ray )
	{
		return null;
	}

	public IHeightMap2D GetClosestHeightMap( Vector2 pos )
	{
		IHeightMap2D best = null;
		float bestDist = float.MaxValue;
		
		foreach ( IHeightMap2D heightmap in _heightmaps )
		{
			if ( heightmap.IsInRange( pos.x )   )
			{
				float h = heightmap.GetHeight( pos.x );
				float d = Math.Abs( h - pos.y );
				
				if ( d < bestDist || best == null )
				{
					best = heightmap;
					bestDist = d;
				}
			}
		}
		
		return best;
	}


	public float GetClosestHeight( Vector2 pos )
	{
		float best = pos.y;
		float bestDist = float.MaxValue;

		foreach ( IHeightMap2D heightmap in _heightmaps )
		{
			if ( heightmap.IsInRange( pos.x )  )
			{
				float h = heightmap.GetHeight( pos.x );
				float d = Math.Abs( h - pos.y );

				if ( d < bestDist )
				{
					best = h;
					bestDist = d;
				}
			}
		}

		return best;
	}

	public IHeightMap2D TestIntersection( Vector2 from, Vector2 to, bool ignoreBackfacingNormals )
	{
		foreach ( IHeightMap2D heightmap in _heightmaps )
		{
			if ( heightmap.IsInRange( from.x ) || heightmap.IsInRange( to.x ) )
			{
				float fromHeight = heightmap.GetHeight( from.x );
				float toHeight 	= heightmap.GetHeight( to.x );

				if (  (fromHeight < from.y) && ( toHeight > to.y )
				    || (fromHeight > from.y) && ( toHeight < to.y ) )
				{
					if ( ignoreBackfacingNormals )
					{
						if ( Vector2.Dot( heightmap.GetNormal( to.x ), (to-from).normalized ) < 0 )
						{
							return heightmap;
						}
					}
					else
					{
						return heightmap;
					}
				}
			}
		}

		return null;
	}
}
