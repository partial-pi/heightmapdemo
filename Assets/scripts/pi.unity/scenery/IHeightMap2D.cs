// --------------------------------------------------------------------------------------------- //
// (c) partial pi 2012-2014
// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// http://creativecommons.org/licenses/by-nc/4.0/deed.en_US
// --------------------------------------------------------------------------------------------- //

using UnityEngine;
using System;


// interface for defining a 2d heightmap
public interface IHeightMap2D
{
	// heights that make up the segments of the heightmap
	Vector2[] Heights
	{
		get;
		set;
	}

	// x position of the heightmap
	float X
	{
		get;
		set;
	}

	// Width of the heigth map the heights will be distributed over this width. 
	float Width
	{
		get;
		set;
	}

	// mesh that represents the heightmap
	Mesh HeightMapMesh
	{
		get;
	}

	// build a heightmap using the given filter
	void  				BuildHeightMap( MeshFilter filter );

	// returns the height at the given position
	float 				GetHeight( float x );

	// check if the given x position is within range of the heightmap
	bool  				IsInRange( float x );

	// returns the normal at the given position
	Vector2 			GetNormal( float x );
}
