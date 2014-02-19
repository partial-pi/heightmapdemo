// --------------------------------------------------------------------------------------------- //
// (c) partial pi 2012-2014
// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// http://creativecommons.org/licenses/by-nc/4.0/deed.en_US
// --------------------------------------------------------------------------------------------- //

using UnityEngine;

using pi.unity.resources;

namespace pi.unity.spriteskeleton
{
	/// <summary>
	/// Captures a bunch of parameters relevant to describe the setup of a gameobject
	/// that acts as the 'skin' of a 'bone' in a sprite skeleton. These parameters include
	/// translation, scale, rotation, sprite used, (default) color and (optionally) 
	/// an identifier of a set of random colors to pick from if so desired. A series
    /// these boneskins make up a (sprite) skin.
	/// </summary>
	[System.Serializable]
	public class SpriteBoneSkin
	{
		/// <summary>
		/// (local) translation of the bone skin
		/// </summary>
		public Vector3  translation;

		/// <summary>
		/// (local) scale of the bone skin
		/// </summary>
		public Vector3  scale;

		/// <summary>
		/// (local) rotation of the bone skin
		/// </summary>
		public Vector3  rotation;

		/// <summary>
		/// Sprite used to render the bone skin
		/// </summary>
		public Sprite 	spriteAsset;

		/// <summary>
		/// Default color applied to the sprite property
		/// </summary>
		public Color	defaultColor;

		/// <summary>
		/// The index of a 'color set' to choose an alternative color from. The color will
		/// be applied to the spriteAsset instead of the defaultColor. If this value
		/// is -1, the default color will used instead.
		/// Note the 'color set' refers to the colorSet property in the SpriteSkinScript.
		/// see cref="pi.unity.spriteskeleton.SpriteSkinScript"/> . 
		/// </summary>
		public int      colorSet = -1;

		/// <summary>
		/// Initializes a new instance of the <see cref="pi.unity.spriteskeleton.BoneSkin"/> class,
		/// setting the scale to 1 and the rotation to identity. 
		/// </summary>
		public SpriteBoneSkin()
		{
			scale    = Vector3.one;
			rotation = Quaternion.identity.eulerAngles;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="pi.unity.spriteskeleton.BoneConfiguration"/> class.
		/// </summary>
		/// <param name="s">default bone sprite.</param>
		/// <param name="t">default transform properties (translation, scale, rotation).</param>
		/// <param name="c">default color of the sprite.</param>
		public SpriteBoneSkin( Sprite s, Transform t, Color c )
		{
			spriteAsset 		= s;
			translation = t.localPosition;
			scale		= t.localScale;
			rotation    = t.rotation.eulerAngles;
			defaultColor		= c;
		}

		/// <summary>
		/// Apply the configuration to the specified renderer.
		/// </summary>
		public void Apply( SpriteRenderer renderer )
		{
			renderer.sprite 							= spriteAsset;
			renderer.gameObject.transform.localPosition = translation;
			renderer.gameObject.transform.localScale    = scale;
			renderer.gameObject.transform.rotation      = Quaternion.Euler( rotation );
			renderer.color								= defaultColor;
		}

		/// <summary>
		/// Apply the configuration to the specified renderer and picks a random color
		/// from the provided colorSet. If the colorSet property of the BoneConfiguration
		/// is not defined, the default color will be assigned.
		/// </summary>
		public void Apply( SpriteRenderer renderer, ColorCollection[] colors, int randomSeed )
		{
			renderer.sprite 							= spriteAsset;
			renderer.gameObject.transform.localPosition = translation;
			renderer.gameObject.transform.localScale    = scale;
			renderer.gameObject.transform.rotation      = Quaternion.Euler( rotation );

			if ( colorSet >= 0 )
			{
				renderer.color = colors[ colorSet ].SelectRandomColor( randomSeed );
			}
			else
			{
				renderer.color = defaultColor;
			}
		}
	}
}