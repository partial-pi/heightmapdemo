// --------------------------------------------------------------------------------------------- //
// (c) partial pi 2012-2014
// Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// http://creativecommons.org/licenses/by-nc/4.0/deed.en_US
// --------------------------------------------------------------------------------------------- //

using UnityEngine;
using System.Collections;
using pi.unity.util;

namespace pi.unity.heightmapdemo
{
	/// <summary>
	/// Humanoid locomotion used in the heightmap demo.
	/// Core features are being able to jump through higher platforms
	/// and lots of buggy physics. This mechanic is driven
	/// by switching the active layer of the humanoid to
	/// colliding / non colliding depending on the state.
	/// </summary>
	public class HumanoidLocomotion : MonoBehaviour 
	{
		// state tracking what action the humanoid is in
		public enum LocomotionState
		{
			Walking,
			Jumping,
			Falling,
		};

		// force applied to the horizontal axis
		public float        horizontalForce		  = 8;

		// max velocity over the horizontal axis. Speed will be clamped
		// to this value.
		public float 		maxHorizontalVelocity = 3;

		// dead zone applying to the current velocity. If
		// the player's velocity is more than this value 
		// the character will turn around
		public float 		facingDeadZone   = 0.05f;

		// max velocity over the vertical axis. Speed will be clamped
		// to this value.
		public float		maxVerticalVelocity   = 10;

		// force applied upwards (ie when the player jumps)
		public float		jumpForce 			  = 5;

		// current state (jump/walk/fall)
		public  LocomotionState _locoState;

		// flag tracking whether or not the humanoid
		// is considered to be jumping
		private bool			_hasReachedJumpVelocity  = false;

		// cached components
		private Rigidbody2D 	_rigidBody;
		private Animator		_animator;
		private BoxCollider2D	_collider;

		/// <summary>
		/// Collects the necessary components and sets the state to falling.
		/// </summary>
		public void Start () 
		{
			_rigidBody = gameObject.GetComponent<Rigidbody2D>();
			_animator  = gameObject.FindComponentByType<Animator>();
			_collider  = gameObject.GetComponent<BoxCollider2D>();

			SetLocomotionState( LocomotionState.Falling );
		}

		/// <summary>
		/// Sets the locomotion new locomotion state and
		/// associated layer.
		/// </summary>
		/// <param name="state">State.</param>
		private void SetLocomotionState( LocomotionState state )
		{
			_locoState = state;

			switch ( _locoState )
			{
			case LocomotionState.Falling:
				gameObject.layer = LayerMask.NameToLayer( "fall" );
				break;
			case LocomotionState.Jumping:
				gameObject.layer = LayerMask.NameToLayer( "jump" );
				_hasReachedJumpVelocity = false;
				break;
			case LocomotionState.Walking:
			default:
				break;
			}
		}

		/// <summary>
		/// Update the input, locomotion state and orientation (facing left/right).
		/// </summary>
		void Update () 
		{
			UpdateInput();

			switch ( _locoState )
			{
			case LocomotionState.Jumping:
				UpdateJumping();
				break;
			default:
				break;
			}

			UpdateFacingDirection();
		}

		// read the input and decide on moving and / or jumping
		private void UpdateInput()
		{
			UpdateMovementInput();

			if ( _locoState == LocomotionState.Walking )
			{
				UpdateJumpingInput();
			}
		}

		// check if the input is set to jump - if so
		// apply a vertical force 
		private void UpdateJumpingInput()
		{
			float v = Input.GetAxis( "Vertical" );
			
			if ( v > 0 )
			{
				_rigidBody.AddForce( new Vector2 ( 0, jumpForce ) );
				SetLocomotionState( LocomotionState.Jumping );
			}
		}

		// reads the horizontal input and applies the force accordingly
		// causing the humanoid to move left or right
		private void UpdateMovementInput()
		{
			float h = Input.GetAxis("Horizontal");
			
			_rigidBody.AddForce( h * new Vector2 ( horizontalForce, 0 ) );

			// clamp the velocity
			if ( Mathf.Abs( _rigidBody.velocity.x ) > maxHorizontalVelocity  )
			{				
				_rigidBody.velocity = ( _rigidBody.velocity.x > 0 ) 
					?  new Vector2( maxHorizontalVelocity,  _rigidBody.velocity.y )
						: new Vector2( -maxHorizontalVelocity,  _rigidBody.velocity.y );
			}
			
			if ( _animator != null )
			{
				_animator.SetFloat( "velocity", Mathf.Abs( _rigidBody.velocity.x ) / maxHorizontalVelocity );
			}
		}

		// callback from engine signalling the humanoid has exited 
		// from an intersection / collision with the given collider. 
		// If that is case and since in this demo we only collide with the
		// ground, we can sort of safely assume the humanoid is falling
		public void OnCollisionExit2D( Collision2D c )
		{
			if ( _locoState == LocomotionState.Walking )
			{
				SetLocomotionState( LocomotionState.Falling );
			}
		}

		// callback from the engine signalling the humanoid has run 
		// into a collision with the given collider. If
		// the humanoid has been falling it implies it has landed
		// on a piece of ground
		public void OnCollisionEnter2D( Collision2D c )
		{
			if ( _locoState == LocomotionState.Falling )
			{
				if ( IsGroundLayer( c.collider.gameObject.layer )  )
				{
					gameObject.layer = c.collider.gameObject.layer;

					SetLocomotionState( LocomotionState.Walking );
				}
			}
		}

		// update the moving state. If the player has reached the necessary 
		// jump speed, the humanoid can start checking to see if the falling 
		// condition has been reached. We need this somewhat strange 
		// check because for the first few frames the humanoid may
		// not have a positive y velocity and it would seem the humanoid is
		// falling.
		private void UpdateJumping()
		{
			if ( !_hasReachedJumpVelocity )
			{
				if ( _rigidBody.velocity.y >= 0.25f )
				{
					_hasReachedJumpVelocity = true;
				}
			}
			else if ( _rigidBody.velocity.y < -0.1f )
			{
				SetLocomotionState( LocomotionState.Falling );
			}
		}

		// checks if the given layer index is a ground layer
		private bool IsGroundLayer( int layer )
		{
			return layer == LayerMask.NameToLayer( "heightmap lv 0" )
				|| layer == LayerMask.NameToLayer( "heightmap lv 1" )  
				|| layer == LayerMask.NameToLayer( "heightmap lv 2" );
		}

		// flips the humanoid as needed depending on velocity
		private void UpdateFacingDirection()
		{
			if ( _rigidBody.velocity.x < -facingDeadZone )
			{
				Vector3 euler = transform.rotation.eulerAngles; 
				euler.y = 0;
				_animator.gameObject.transform.rotation = Quaternion.Euler( euler );
			}
			else if ( _rigidBody.velocity.x > facingDeadZone  )
			{
				Vector3 euler = transform.rotation.eulerAngles; 
				euler.y = -180;
				_animator.gameObject.transform.rotation = Quaternion.Euler( euler );
			}
		}
	}
}
