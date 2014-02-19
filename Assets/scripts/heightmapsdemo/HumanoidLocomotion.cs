using UnityEngine;
using System.Collections;
using pi.unity.util;

namespace pi.unity.heightmapdemo
{
	public class HumanoidLocomotion : MonoBehaviour 
	{
		public enum LocomotionState
		{
			Walking,
			Jumping,
			Falling,
			Landing
		};

		public float        horizontalForce		  = 8;
		public float 		maxHorizontalVelocity = 3;
		public float 		orientationDeadZone   = 0.05f;

		public float		maxVerticalVelocity   = 10;
		public float		jumpForce 			  = 5;
		public float		landingDuration 	  = 0.5f;

		public  LocomotionState _locoState;

		private Rigidbody2D 	_rigidBody;
		private Animator		_animator;
		private BoxCollider2D	_collider;
		private Color			_gizmoColor = Color.white;
		private bool			_hasReachedJumpVelocity  = false;

		// Use this for initialization
		void Start () 
		{
			_rigidBody = gameObject.GetComponent<Rigidbody2D>();
			_animator  = gameObject.FindComponentByType<Animator>();
			_collider  = gameObject.GetComponent<BoxCollider2D>();

			SetLocomotionState( LocomotionState.Falling );
		}

		void SetLocomotionState( LocomotionState state )
		{
			_locoState = state;

			switch ( _locoState )
			{
			case LocomotionState.Falling:
				gameObject.layer = LayerMask.NameToLayer( "fall" );
				//Debug.Log("fall");
				break;
			case LocomotionState.Jumping:
				gameObject.layer = LayerMask.NameToLayer( "jump" );
				//Debug.Log("jump");
				_hasReachedJumpVelocity = false;
				break;
			case LocomotionState.Landing:
			case LocomotionState.Walking:
			default:
				break;
			}
		}

		// Update is called once per frame
		void Update () 
		{
			_gizmoColor = Color.white;

			UpdateInput();

			switch ( _locoState )
			{
			case LocomotionState.Falling:
				UpdateFalling();
				break;
			case LocomotionState.Jumping:
				UpdateJumping();
				break;
			case LocomotionState.Landing:
				UpdateLanding();
				break;
			case LocomotionState.Walking:
				UpdateWalking();
				break;
			default:
				break;
			}

			UpdateOrientation();
		}

		private void UpdateInput()
		{
			if ( _locoState != LocomotionState.Landing )
			{
				UpdateMovementInput();

				if ( _locoState == LocomotionState.Walking )
				{
					UpdateJumpingInput();
				}
			}
		}

		private void UpdateJumpingInput()
		{
			float v = Input.GetAxis( "Vertical" );
			
			if ( v > 0 )
			{
				_rigidBody.AddForce( new Vector2 ( 0, jumpForce ) );
				SetLocomotionState( LocomotionState.Jumping );
			}
		}

		private void UpdateMovementInput()
		{
			float h = Input.GetAxis("Horizontal");
			
			_rigidBody.AddForce( h * new Vector2 ( horizontalForce, 0 ) );
			
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

		private void UpdateFalling()
		{
		}

		/*
		public void OnDrawGizmos()
		{
			if ( _collider != null )
			{
				float rayCastStartX = gameObject.transform.position.x + _collider.center.x;
				float rayCastStartY = gameObject.transform.position.y + _collider.center.y + _collider.size.y * 0.5f;
				Vector2 rayCastStart = new Vector2( rayCastStartX, rayCastStartY );

				Gizmos.color = _gizmoColor;
				Gizmos.DrawLine( new Vector3( rayCastStart.x, rayCastStart.y, 0 )
				               , new Vector3( rayCastStart.x, rayCastStartY - 1, 0 ) );
			}
		}
*/
		private void UpdateWalking()
		{
			/*int   layerMask 	= 1 << gameObject.layer;
			float rayCastStartX = gameObject.transform.position.x + _collider.center.x;
			float rayCastStartY = gameObject.transform.position.y + _collider.center.y - _collider.size.y * 0.5f;
			Vector2 rayCastStart = new Vector2( rayCastStartX, rayCastStartY );
		
			RaycastHit2D[] hit = Physics2D.RaycastAll( rayCastStart, -Vector2.up, 1, layerMask ); 
		
			_gizmoColor = Color.red;

			bool hasHitGround = false;

			if ( hit != null && hit.Length > 0  )
			{

				for ( int i = 0; i < hit.Length; ++i )
				{
					if ( hit[ i ].collider != null && hit[i].collider.gameObject.tag == "Ground" )
					{
						_gizmoColor  = Color.green;
						hasHitGround = true;
						break;
					}
				}
			}

			if ( !hasHitGround )
			{
				SetLocomotionState( LocomotionState.Falling );
			}*/
		}

		public void OnCollisionExit2D( Collision2D c )
		{
			//if ( c.collider.gameObject.tag == "Ground" && c.collider.gameObject.layer == gameObject.layer )
			if ( _locoState == LocomotionState.Walking )
			{
				SetLocomotionState( LocomotionState.Falling );
				_gizmoColor = Color.red;
			}
		}

		private bool HitsBottomOfCollider( ContactPoint2D[] points )
		{
			float bottom = 0.1f + gameObject.transform.position.y + _collider.center.y - _collider.size.y * 0.5f;

			//Debug.Log("bottom = " + bottom  + ", got : " + gameObject.transform.position );

			for ( int i = 0; i < points.Length; i++ )
			{
				if ( points[i].point.y <= bottom )
				{
			//		Debug.Log("contact at = " + points[i].point.y 
			//		          + ", with " + LayerMask.LayerToName( points[i].collider.gameObject.layer ) );

					return true;
				}
			}

			return false;
		}

		public void OnCollisionEnter2D( Collision2D c )
		{
			if ( _locoState == LocomotionState.Falling )
			{
				if ( IsGroundLayer( c.collider.gameObject.layer ) /*&& HitsBottomOfCollider( c.contacts )*/ )
				{
					gameObject.layer = c.collider.gameObject.layer;
					//Debug.Log(LayerMask.LayerToName(c.collider.gameObject.layer) );

					SetLocomotionState( LocomotionState.Walking );
				}
			}
		}

		/*public void OnCollisionStayD( Collision2D c )
		{
			if ( c.collider.gameObject.tag == "Ground" && c.collider.gameObject.layer == gameObject.layer )
			{
				_gizmoColor = Color.green;
			}
		}*/

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

		private void UpdateLanding()
		{
		}

		private bool IsGroundLayer( int layer )
		{
			return layer == LayerMask.NameToLayer( "heightmap lv 0" )
				|| layer == LayerMask.NameToLayer( "heightmap lv 1" )  
				|| layer == LayerMask.NameToLayer( "heightmap lv 2" );
		}

		private void UpdateOrientation()
		{
			if ( _rigidBody.velocity.x < -orientationDeadZone )
			{
				Vector3 euler = transform.rotation.eulerAngles; 
				euler.y = 0;
				_animator.gameObject.transform.rotation = Quaternion.Euler( euler );
			}
			else if ( _rigidBody.velocity.x > orientationDeadZone  )
			{
				Vector3 euler = transform.rotation.eulerAngles; 
				euler.y = -180;
				_animator.gameObject.transform.rotation = Quaternion.Euler( euler );
			}
		}
	}
}
