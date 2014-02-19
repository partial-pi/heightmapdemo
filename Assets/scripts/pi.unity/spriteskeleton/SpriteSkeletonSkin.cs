// --------------------------------------------------------------------------------------------- //
// (c) partial pi 2012-2014
// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// http://creativecommons.org/licenses/by-nc/4.0/deed.en_US
// --------------------------------------------------------------------------------------------- //

using UnityEngine;
using pi.unity.util;

#if UNITY_EDITOR
using UnityEditor;
#endif

using pi.unity.resources;

namespace pi.unity.spriteskeleton
{
	/// <summary>
	/// Collection of sprite bone skins that collectively make up a sprite skin
	/// </summary>
	[System.Serializable]
	public class SpriteSkeletonSkin
	{
		/// <summary>
		///  Name of the configuration, a fancy or mundane human readable name for in the editor
		/// </summary>
		public string     name;

		/// <summary>
		/// The bone configuration set that collectively make up this skeleton 
		/// </summary>
		public SpriteBoneSkin[] boneSkins;

		/// <summary>
		/// Applies a random bone skin to the given renderer based on the given randomseed.
		/// </summary>
		public void ApplyRandom( SpriteRenderer renderer, int randomSeed )
		{
			Random.seed = randomSeed;
			boneSkins[ Random.Range( 0, boneSkins.Length ) ].Apply( renderer ); 
		}

		/// <summary>
		/// Applies a random bone skin and color from the given color collection to the given renderer 
		/// based on the given randomseed.
		/// </summary>
		public void ApplyRandom( SpriteRenderer renderer, ColorCollection[] colors, int randomSeed )
		{
#if UNITY_EDITOR
			Assert.NotNullOrEmpty( boneSkins, "ApplyRandom to an empty boneSkinSet ("+ name +").");
			Assert.NotNullOrEmpty( colors, "ApplyRandom with an empty or null color set to " + name );
#endif
			Random.seed = randomSeed;
			boneSkins[ Random.Range( 0, boneSkins.Length ) ].Apply( renderer, colors, randomSeed ); 
		}

		/// <summary>
		/// Applies the bone skin defined by the index to the renderer.
		/// </summary>
		public void Apply( SpriteRenderer renderer, int index )
		{
			boneSkins[ index ].Apply( renderer ); 
		}

#if UNITY_EDITOR
		/// <summary>
		/// Checks if the given sprite is already in the  boneskin array property
		/// </summary>
		public bool ContainsSprite( Sprite s )
		{
			foreach ( SpriteBoneSkin config in boneSkins )
			{
				if( ( config.spriteAsset == null && s == null )
				   || ( config.spriteAsset != null 
				    && s != null
				    && config.spriteAsset.GetInstanceID() == s.GetInstanceID() ) )
				{
					return true;
				}
			}
			
			return false;
		}

	
		/// <summary>
		/// Adds the bone configuration defined by the given parameters to the boneskin
		/// array property.
		/// </summary>
		public void AddBoneConfiguration( Sprite s, Transform t, Color c )
		{
			if ( boneSkins == null )
			{
				boneSkins = new SpriteBoneSkin[1];
				boneSkins[ 0 ] = new SpriteBoneSkin( s, t, c  );
			}
			else
			{
				SpriteBoneSkin[] newConfig = new SpriteBoneSkin[1];
				
				ArrayUtility.AddRange( ref newConfig, boneSkins );
				
				// note array utility moves the original sprites from 0, 1, 2 ... N -> 1,2 .. N + 1
				// so we add the new config to 0 
				newConfig[ 0 ] = new SpriteBoneSkin( s, t, c );
				
				boneSkins = newConfig;
			}
		}
#endif
	}
}