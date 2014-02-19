// --------------------------------------------------------------------------------------------- //
// (c) partial pi 2012-2014
// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// http://creativecommons.org/licenses/by-nc/4.0/deed.en_US
// --------------------------------------------------------------------------------------------- //

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using pi.unity.util;

namespace pi.unity.util
{
	/// <summary>
	/// Game object extensions primarily for helping to find components in
	/// the transform.
	/// </summary>
	public static class GameObjectExtensions
	{
		/// <summary>
		/// Finds the child object with the game name using on a slash separated path. Eg
		/// root/layer1/child1 tries to find the child1 game object attached
		/// to the layer1 game object attached to the root object
		/// </summary>
		/// <returns>The child object.</returns>
		public static GameObject FindChildObjectByName( this GameObject targetGameObject, string target )
		{
#if UNITY_EDITOR
			Assert.NotNullOrEmpty( target, "Will not find any children by name if no valid name is provided." );
#endif
			string[] path = target.IndexOf('/') >= 0 
				? target.Split( '/' )
			: new string[] { target };
			
			int pathIndex = 0;
			Transform targetTransform = targetGameObject.transform.Find( path[ 0 ] );
			
			while ( targetTransform != null && ++pathIndex < path.Length )
			{
				targetTransform = targetTransform.Find( path[ pathIndex ] );
			}
			
			return targetTransform != null ? targetTransform.gameObject : null;
		}

		/// <summary>
		/// Finds first child containing a component of type T. Does not search the game object or children recursively.
		/// </summary>
		/// <returns>A valid component or null if there was no child with the matching
		/// type.</returns>
		public static T FindChildComponentByType<T>( this GameObject targetGameObject ) where T : Component
		{
			T result = null;

			for ( int i = 0; i < targetGameObject.transform.childCount; ++i )
			{
				result = targetGameObject.transform.GetChild( i ).GetComponent<T>();

				if ( result != null )
				{
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Returns a list of all components of type T in the gameobject and all its children
		/// (and grandchilren, and great grandchildren and so on ).
		/// </summary>
		/// <returns>A non null list of all components found.</returns>
		public static List<T> FindAllComponentsByType<T>( this GameObject targetGameObject ) where T : Component
		{
			return targetGameObject.FindAllComponentsByType<T>( new List<T>() );
		}

		/// <summary>
		/// Finds all components of type T in the gameobject and all its descendants
		/// and puts them in the provided list.
		/// </summary>
		/// <returns>The same list as provided.</returns>
		public static List<T> FindAllComponentsByType<T>( this GameObject targetGameObject, List<T> result ) where T : Component
		{
	#if UNITY_EDITOR
			Assert.NotNull( result );
	#endif

			T current = targetGameObject.GetComponent<T>();

			if ( current != null )
			{
				result.Add( current );
			}

			for ( int i = 0; i < targetGameObject.transform.childCount; ++i )
			{
				targetGameObject.transform.GetChild( i ).gameObject.FindAllComponentsByType<T>( result );
			}

			return result;
		}

		/// <summary>
		/// Recursively tries to find the first component of type T in the gameobject or any
		/// of its children
		/// </summary>
		/// <returns>The first component of type T or null if no such component was found.</returns>
		public static T FindComponentByType<T>( this GameObject targetGameObject ) where T : Component
		{
			T result = targetGameObject.GetComponent<T>();

			if ( result == null )
			{
				for ( int i = 0; i < targetGameObject.transform.childCount; ++i )
				{
					result = targetGameObject.transform.GetChild( i ).gameObject.FindComponentByType<T>();
					
					if ( result != null )
					{
						break;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Finds the all child objects that match the given function and return them in a list.
		/// </summary>
		/// <returns>A non null but possibly empty list of all matching child objects.</returns>
		public static List<GameObject> FindMatchingChildObjects( this GameObject root, Func< GameObject, bool > function )
		{
			return root.FindMatchingChildObjects( function, new List<GameObject>() );  
		}

		/// <summary>
		/// Finds the all child objects that match the given function and put them in the provided list.
		/// </summary>
		/// <returns>The same list as provided.</returns>
		public static List<GameObject> FindMatchingChildObjects( this GameObject root
		                                               , Func< GameObject, bool > function
		                                               , List<GameObject> result  )
		{
#if UNITY_EDITOR
			Assert.NotNull( result, "Cannot store results in an empty list" );
#endif
			for ( int i = 0; i < root.transform.childCount; ++i )
			{
				GameObject child = root.transform.GetChild( i ).gameObject;

				if ( function.Invoke( child ) )
				{
					result.Add( child );
				}

				child.FindMatchingChildObjects( function, result );
			}

			return result;
		}

		/// <summary>
		/// Finds the descendant of the gameobject that match the given function and add
		/// the component of type T to a result list.
		/// </summary>
		/// <returns>A list of components of which the gameobject matches the given functions.</returns>
		public static List<T> FindChildComponents<T>( this GameObject root	
		                                             , Func< GameObject, bool > function ) where T : Component 
		{
			return FindChildComponents<T>( root, function, new List<T>() );
		}

		// <summary>
		/// Finds the descendant of the gameobject that matches the given function and adds
		/// the component of type T to a result list.
		/// </summary>
		/// <returns>The provided result list with 0 or more components.</returns>
		public static List<T> FindChildComponents<T>( this GameObject root
		                                                , Func< GameObject, bool > function
		                                                , List<T> result  ) where T : Component 
		{
			for ( int i = 0; i < root.transform.childCount; ++i )
			{
				GameObject child = root.transform.GetChild( i ).gameObject;
				
				if ( function.Invoke( child ) )
				{
					result.Add( child.GetComponent<T>()  );
				}
				
				child.FindChildComponents<T>( function, result );
			}
			
			return result;
		}

		/// <summary>
		/// Determines if the root object is a parent to the given child.
		/// </summary>
		/// <returns><c>true</c> if there is a path from child to root via its transforms; otherwise, <c>false</c>.</returns>
		public static bool HasChild( this GameObject root, GameObject child )
		{
#if UNITY_EDITOR
			Assert.NotNull( root );
			Assert.NotNull( child );
#endif
			while ( child != null && child != root )
			{
				Transform parentTransform  = child.transform.parent;

				if ( parentTransform == null )
				{
					child = null;
				}
				else
				{
					child = parentTransform.gameObject;
				}
			}

			return child == root;
		}

		/// <summary>
		/// Creates a string path from the child to the root object (assuming the child is a descendant
		/// of root) seperating the names of each gameobject on that path by the given separator
		/// </summary>
		/// <returns>The path to child.</returns>
		/// <param name="root">Root.</param>
		/// <param name="child">Child.</param>
		public static string CreatePathToChild( this GameObject root, GameObject child, string separator )
		{
#if UNITY_EDITOR
			Assert.IsTrue( root.HasChild( child ), "Cannot create a path from root to a child that is not a descendant." );
#endif

			string result = child.name;
			
			while ( child != null 
			       && child != root 
			       && child.transform.parent != null
			       && child.transform.parent.gameObject != root )
			{
				result = child.transform.parent.gameObject.name + separator + result;
				child = child.transform.parent.gameObject;
			}
			
			return result;
		}
	}
}
