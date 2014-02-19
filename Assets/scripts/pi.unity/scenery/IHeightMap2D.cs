
using UnityEngine;
using System;

public interface IHeightMap2D
{
	Vector2[] Heights
	{
		get;
		set;
	}
	
	float X
	{
		get;
		set;
	}
	
	float Width
	{
		get;
		set;
	}

	Mesh HeightMapMesh
	{
		get;
	}

	void  				BuildHeightMap( MeshFilter filter );
	float 				GetHeight( float x );
	bool  				IsInRange( float x );
	Vector2 			GetNormal( float x );
}
