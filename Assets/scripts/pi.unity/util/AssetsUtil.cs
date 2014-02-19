#if UNITY_EDITOR
using System;
using System.IO;

using UnityEngine;
using UnityEditor;

public static class AssetsUtil
{
	/// <summary>
	/// Gets the current path based on the selected object.
	/// This is lifted directly from the unity forums but I didn't write the link down.
	/// So credits to the anonymous forum contributor.
	/// </summary>
	/// <returns>The current path.</returns>
	public static string GetCurrentPath()
	{
		string path = "Assets";
		
		foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
		{
			path = AssetDatabase.GetAssetPath(obj);
			
			if (File.Exists(path))
			{
				path = Path.GetDirectoryName(path);
			}
			
			break;
		}

		return path;
	}
}
#endif