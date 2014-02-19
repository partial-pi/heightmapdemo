// --------------------------------------------------------------------------------------------- //
// (c) partial pi 2012-2014
// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// http://creativecommons.org/licenses/by-nc/4.0/deed.en_US
// --------------------------------------------------------------------------------------------- //

using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using pi.unity.util;
using pi.unity.resources;

namespace pi.unity.spriteskeleton
{
	/// <summary>
	/// Scriptable object that wraps a SpriteSkeletonSkin in a Scriptable object
    /// exposing it to the unity editor. 
	/// </summary>
	public class SpriteSkeletonSkinAsset : ScriptableObject
	{

#if UNITY_EDITOR
		/// <summary>
		///  Allows creation of a sprite skin via the context menu in the editor
		/// </summary>
		[MenuItem("Assets/Create/Sprite skin")]
		public static void CreateInstance()
		{
			SpriteSkeletonSkinAsset state = ScriptableObject.CreateInstance<SpriteSkeletonSkinAsset>();
			AssetDatabase.CreateAsset( state, 
			                          AssetDatabase.GenerateUniqueAssetPath( AssetsUtil.GetCurrentPath() + "/sprite skin.asset" ) );
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = state;
		}
#endif
		/// <summary>
		/// List of all sprite bone options making up this skeleton 
		/// </summary>
		public SpriteSkeletonSkin[] skeletonBones;

		/// <summary>
		/// Collection of colors from which the bones may select a color to apply to sprite bone skin.
		/// </summary>
		public ColorCollection[] 	colorOptions;

		public SpriteSkeletonSkinAsset()
		{
		}

#if UNITY_EDITOR
		/// <summary>
		/// Initializes a new instance of the <see cref="pi.unity.spriteskeleton.SpriteSkeletonSkinAsset"/> class.
		/// Calls CreateFromSkeleton using the given skeletonRoot
		/// </summary>
		/// <param name="skeletonRoot">Skeleton root.</param>
		public SpriteSkeletonSkinAsset( GameObject skeletonRoot )
		{
			CreateFromSkeleton( skeletonRoot );
		}
#endif

		/// <summary>
		/// Applies random permutation of this skin to the set of renderers based on the given randomSeed.
		/// </summary>
		public void ApplyRandom( System.Collections.Generic.IEnumerable<SpriteRenderer> renderers, int randomSeed )
		{
			int i = 0;

			if ( colorOptions != null && colorOptions.Length > 0 )
			{
				foreach ( SpriteRenderer renderer in renderers )
				{
					if ( skeletonBones[ i ] != null 
					    && skeletonBones[ i ].boneSkins != null
					    && skeletonBones[ i ].boneSkins.Length > 0 )
					{
						skeletonBones[ i++ ].ApplyRandom( renderer, colorOptions, randomSeed );
					}
				}
			}
			else
			{
				foreach ( SpriteRenderer renderer in renderers )
				{
					if ( skeletonBones[ i ] != null 
					    && skeletonBones[ i ].boneSkins != null
					    && skeletonBones[ i ].boneSkins.Length > 0 )
					{
						skeletonBones[ i++ ].ApplyRandom( renderer, randomSeed );
					}
				}
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Creates a skin based on the skeleton defined from the given root.
		/// The root is assumed to have an Animator and all subsequent child gameobjects
		/// are with sprite renders are considered bones. This method iterates through
		/// all bones and create a skin from the current settings of these bones.
		/// </summary>
		/// <param name="root">Root.</param>
		public void CreateFromSkeleton( GameObject root )
		{
			Assert.NotNull( root );
			Assert.HasComponent<Animator>( root, "ApplyRandom, parameter needs to have an Animator component" ); 
			
			List<GameObject> bones = root.FindMatchingChildObjects( x => x.GetComponent<SpriteRenderer>() != null );
			
			if ( bones.Count > 0 )
			{
				skeletonBones = new SpriteSkeletonSkin[ bones.Count ];
				
				for ( int i = 0; i < bones.Count; ++i )
				{
					skeletonBones[ i ] = new SpriteSkeletonSkin()
					{
						name = root.CreatePathToChild( bones[ i ], "/" ),
						boneSkins = new SpriteBoneSkin[ 1 ]
					};
					
					SpriteRenderer renderer = bones[ i ].GetComponent<SpriteRenderer>();
					
					skeletonBones[ i ].boneSkins[ 0 ] 
					= new SpriteBoneSkin( renderer.sprite, bones[ i ].transform, renderer.color );
					
					Debug.Log ("[Merge sprite skin] Added " + skeletonBones[ i ].name + " (" + i + ")." ); 
				}
			}
		}

		/// <summary>
		/// Merges the skin defined by the given skeleton with the current skin (much like a union).
		/// </summary>
		public void Merge( GameObject skeletonRoot )
		{
			Assert.NotNull( skeletonRoot );
			Assert.HasComponent<Animator>( skeletonRoot, "ApplyRandom, parameter needs to have an Animator component" ); 

			foreach ( SpriteSkeletonSkin mapping in skeletonBones )
			{
				GameObject bone = skeletonRoot.FindChildObjectByName( mapping.name );
				
				if ( bone != null )
				{
					SpriteRenderer renderer = bone.GetComponent<SpriteRenderer>();
					
					if ( renderer != null && !mapping.ContainsSprite( renderer.sprite ) )
					{
						mapping.AddBoneConfiguration( renderer.sprite, bone.transform, renderer.color ); 
						Debug.Log ( "[Merge] Added sprite: " + renderer.sprite + " to " + mapping.name );
					}
				}
			}
		}

		/// <summary>
		/// Applies random permutation of this skin to the set of renderers based on the given randomSeed.
		/// </summary>
		public void ApplyRandom( GameObject root, int randomSeed )
		{
			Assert.NotNull( root );
			Assert.HasComponent<Animator>( root, "ApplyRandom, parameter needs to have an Animator component" ); 

			if ( skeletonBones != null )
			{
				foreach ( SpriteSkeletonSkin mapping in skeletonBones )
				{
					GameObject child = root.FindChildObjectByName( mapping.name );
					
					if ( child != null )
					{
						if ( colorOptions != null && colorOptions.Length > 0 )
						{
							mapping.ApplyRandom( child.GetComponent<SpriteRenderer>(), colorOptions, randomSeed );
						}
						else
						{
							mapping.ApplyRandom( child.GetComponent<SpriteRenderer>(), randomSeed );
						}
					}
				}
			}
			else
			{
				Debug.LogWarning( "Apply random skin configuration: no bonemappings defined. No skin will be applied." );
			}
		}
#endif
	}
}