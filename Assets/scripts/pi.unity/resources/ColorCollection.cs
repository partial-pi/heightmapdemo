// --------------------------------------------------------------------------------------------- //
// (c) partial pi 2012-2014
// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// http://creativecommons.org/licenses/by-nc/4.0/deed.en_US
// --------------------------------------------------------------------------------------------- //
using System.Collections;

using UnityEngine;

using pi.unity.util;

namespace pi.unity.resources
{
	/// <summary>
	/// Collection of colors that allows you to give it a name and 
    /// ask it to give you a random color.
	/// </summary>
	[System.Serializable]
	public class ColorCollection
	{
		/// <summary>
		/// Name of the colorset, handy for identifying the intention of 
		/// the colors in the unity editor
		/// </summary>
		public string  name;

		/// <summary>
		/// The actual colors that make this set
		/// </summary>
		public Color[] colors;
	
		/// <summary>
		/// Convenience method returning a random color based on a provided 
		/// seed. 
		/// </summary>
		public Color SelectRandomColor( int seed )
		{
#if UNITY_EDITOR
			Assert.NotNullOrEmpty( colors, "[ColorSet] ColorSet " + name + " is empty add some colors first before asking for a random color." );
#endif
			Random.seed = seed;
			return colors[Random.Range( 0, colors.Length )];
		}
	}
}