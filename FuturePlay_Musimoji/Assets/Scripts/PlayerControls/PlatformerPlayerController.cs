using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlatformerPlayerController : MonoBehaviour
{
	public bool debugMessages = false;
	
    [SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private string m_LadderTag = "Ladder";
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings

	private const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	[SerializeField] private bool m_Grounded;            // Whether or not the player is grounded.
	[SerializeField] private bool m_Ladder;				// Whether or not the player is on a ladder.
	private const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private float startGravityScale;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private Vector2 moveDir;
	private bool jumpRequested;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		startGravityScale = m_Rigidbody2D.gravityScale;

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);

		bool wasOnLadder = m_Ladder;
		m_Ladder = false;
		// Check for ladders first
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].CompareTag(m_LadderTag))
			{
				m_Ladder = true;
				if (!wasOnLadder)
				{
					m_Rigidbody2D.gravityScale = 0;
					m_Rigidbody2D.isKinematic = true;
				}
				continue;
			}
			
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}

		if (wasOnLadder && !m_Ladder)
		{
			m_Rigidbody2D.gravityScale = startGravityScale;
			m_Rigidbody2D.isKinematic = false;
		}
		
		Move();
	}

	#region Inputs

	public void OnMove(InputAction.CallbackContext callbackContext)
	{
		moveDir = callbackContext.ReadValue<Vector2>();
		
		if(moveDir.magnitude>0.01f && debugMessages) Debug.Log($"Move input {moveDir}");
	}
	
	public void OnJump(InputAction.CallbackContext callbackContext)
	{
		if (callbackContext.started)
		{
			if(debugMessages) Debug.Log($"Button 1 started");
			jumpRequested = true;
		}
        
		if (callbackContext.canceled)
		{
			if(debugMessages) Debug.Log($"Button 1 cancelled");
			jumpRequested = false;
		}
	}

	#endregion


	public void Move()
	{
		// If the player should jump...
		if ((m_Ladder || m_Grounded) && jumpRequested)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
		
		//ladder control
		if (m_Ladder)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(moveDir.x * 10f, moveDir.y * 10f);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (moveDir.x > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (moveDir.x < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		
		//only control the player if grounded or airControl is turned on
		if (!m_Ladder && m_Grounded || m_AirControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(moveDir.x * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (moveDir.x > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (moveDir.x < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
