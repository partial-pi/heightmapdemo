// --------------------------------------------------------------------------------------------- //
// (c) partial pi 2012-2014
// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// http://creativecommons.org/licenses/by-nc/4.0/deed.en_US
// --------------------------------------------------------------------------------------------- //


#if UNITY_EDITOR

using UnityEngine;

namespace pi.unity.util
{
	/// <summary>
	/// Assert exception. This Exception will be thrown when an Assert fails.
	/// </summary>
	public class AssertException : System.Exception
	{
		public AssertException()
			: base( "Assertion failed" )
		{
		}

		public AssertException( string message )
			: base( message )
		{
		}
	}

	/// <summary>
	/// Utility class used to check preconditions. Only available in editor builds.
	/// </summary>
	public static class Assert
	{
		/// <summary>
		/// Requires b to be true or throws an assert exception with the given message.
		/// </summary>
		public static void IsTrue( bool b, string message )
		{
			if ( !b )
			{
				throw new AssertException( "Assertion failed, IsTrue. " + message );
			}
		}

		/// <summary>
		/// Requires b to be true or throws an assert exception 
		/// </summary>
		public static void IsTrue( bool b )
		{
			IsTrue ( b, "" );
		}

		/// <summary>
		/// Requires the given object not to be null or throws an assert exception with the given message.
		/// </summary>
		public static void NotNull( object o, string message )
		{
			if ( o == null )
			{
				throw new AssertException( "Assertion failed, NotNull. " + message );
			}
		}

		/// <summary>
		/// Requires the given object not to be null or throws an assert exception.
		/// </summary>
		public static void NotNull( object o )
		{
			NotNull( o, "" );
		}

		/// <summary>
		/// Requires the array of the given type not to be null and containing 1 or more elements
	    /// or throws an assert exception with the given message.
		/// </summary>
		public static void NotNullOrEmpty<T>( T[] objectArray, string message )
		{
			if ( objectArray == null || objectArray.Length == 0 )
			{
				throw new AssertException( "Assertion failed, NotNullOrEmpty (array). " + message );
			}
		}

		/// Requires the array of the given type not to be null and containing 1 or more elements
		/// or throws an assert exception.
		public static void NotNullOrEmpty<T>( T[] objectArray )
		{
			NotNullOrEmpty<T>( objectArray, "" );
		}

		/// <summary>
		/// Requires game object to have a component of the given type
		/// or throws an assert exception with the given message.
		/// </summary>
		public static void HasComponent<T>( GameObject o, string message ) where T : Component
		{
			if ( o.GetComponent<T>() == null )
			{
				throw new AssertException( "Assertion failed, HasComponent< " + typeof(T) + ">. " + message );
			}
		}

		/// <summary>
		/// Requires game object to have a component of the given type
		/// or throws an assert exception.
		/// </summary>
		public static void HasComponent<T>( GameObject o )where T : Component
		{
			HasComponent<T>( o, "" );
		}

		/// <summary>
		/// Requires given string to be not null or empty
		/// or throws an assert exception.
		/// </summary>
		public static void NotNullOrEmpty( string s  )
		{
			NotNullOrEmpty( s, "" );
		}

		/// <summary>
		/// Requires given string to be not null or empty
		/// or throws an assert exception with the given message.
		/// </summary>
		public static void NotNullOrEmpty( string s, string message )
		{
			if ( string.IsNullOrEmpty( s ) )
			{
				throw new AssertException( "Assertion failed, NotNullOrEmpty(string). " + message );
			}
		}
	}
}

#endif