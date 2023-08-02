using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Platformer_PlayerMovementBasic : MonoBehaviour
{
    public bool debugMessages = false;

    [SerializeField] private BoxCollider2D playerCollider;

    [SerializeField] private LayerMask collisionLayers;

    // [SerializeField] private ContactFilter2D contactFilter;

    [Header("Settings"), Tooltip("Fall faster")] 
    public float gravity = 9.8f;
    [Tooltip("Move left & right")]
    public float moveSpeed = 5;
    [Tooltip("Vertical velocity while jump button is held")]
    public float jumpPower = 8f;
    [Tooltip("How long jump will remain active while holding button")]
    public float jumpDuration = 0.2f;
    
    [Tooltip("If true, pressing 'up' on the joystick will trigger 'jump'")]
    public bool jumpWithUpDirection = false;
    
    [Range(0,4), Tooltip("Assign 'jump' to a button, 0=unassigned")]
    public int jumpButton = 0;
    
    private Vector2 moveStep;
    [SerializeField] private Vector2 velocity;
    [SerializeField] private Vector2 moveDir;
    private float aim;
    [SerializeField] private Vector2 aimDir;
    [SerializeField] private Vector2 mousePos;

    private bool jumpRequested, jumpExpended, directionJumpReady = true;
    
    private float jumpTimer;

    [SerializeField] private bool isGrounded = false;

    private bool anyKey_isDown, button1_isDown, button2_isDown, button3_isDown, button4_isDown;

    public Action Button1_OnDown, Button2_OnDown, Button3_OnDown, Button4_OnDown;
    public Action Button1_OnUp, Button2_OnUp, Button3_OnUp, Button4_OnUp;

    public int PlayerID { get; private set; }

    public void SetID(int playerID)
    {
        PlayerID = playerID;
    }

    private void OnEnable()
    {
        SubscribeButtonsToActions(true);
    }

    private void OnDisable()
    {
        SubscribeButtonsToActions(false);
    }

    #region Gameplay

    private void LateUpdate()
    {
        SetVelocity();
        Move();
        Aim();
    }

    private void FixedUpdate()
    {
        
    }

    private void SetVelocity()
    {
        velocity.x = moveDir.x * (moveSpeed * Time.deltaTime);
        if(!isGrounded) velocity.y -= gravity * Time.deltaTime;
        
        if (jumpWithUpDirection && !jumpExpended && moveDir.y > 0) OnJumpDown();

        if (jumpWithUpDirection && moveDir.y <= 0) OnJumpUp();

        Jump();
    }

    private void Move()
    {
        if(MoveBlocked(velocity)) return;
        transform.Translate(velocity, Space.World);
    }

    private bool MoveBlocked(Vector2 moveDir)
    {
        var currentPosition = (Vector2)transform.position + playerCollider.offset;
        
        // var nonPlayerCollisionObjects = new List<GameObject>();
        var nonPlayerCollisionCount = 0;
        var collisions = Physics2D.BoxCastAll(currentPosition, playerCollider.size, 0f, moveDir,
            Vector2.Distance(currentPosition, currentPosition + moveDir), collisionLayers);

        foreach (var c in collisions)
        {
            if(c.transform.gameObject==this.gameObject) continue;
            
            nonPlayerCollisionCount++;
            
            //Checks if the collision was below the player & grounds them if so. This is a very simple grounding check
            if (!(c.point.y < transform.position.y)) continue;

            OnGrounded();
        }

        return nonPlayerCollisionCount > 0;
    }

    private void OnGrounded()
    {
        isGrounded = true;
        velocity.y = 0f;
        
        if(!jumpRequested) jumpExpended = false;
    }

    private void Jump()
    {
        if (!jumpRequested) return;

        if (jumpExpended) return;

        isGrounded = false;

        velocity.y = jumpPower * Time.deltaTime;

        jumpTimer += Time.deltaTime;

        if (jumpTimer < jumpDuration) return;

        jumpTimer = 0f;

        jumpExpended = true;
    }

    private void OnJumpDown()
    {
        if(!jumpExpended) jumpRequested = true;
    }

    private void OnJumpUp()
    {
        jumpRequested = false;
        jumpExpended = !isGrounded;
    }

    private void Aim()
    {
        if (aimDir != Vector2.zero)
        {
            var angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg + 90;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Debug.DrawLine(transform.position, transform.position + transform.forward * 2, Color.yellow);
        }
    }

    #endregion

    #region Move & Look Inputs

    //Send Message
    private void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
    }

    //Unity Event
    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        moveDir = callbackContext.ReadValue<Vector2>();
    }

    #endregion

    #region Button Events
    
    public void OnAnyKey(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if(debugMessages) Debug.Log($"AnyKey started");
            anyKey_isDown = true;
        }
        
        if (callbackContext.canceled)
        {
            if(debugMessages) Debug.Log($"AnyKey cancelled");
            anyKey_isDown = false;
        }
    }

    public void OnButton1(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if(debugMessages) Debug.Log($"Button 1 started");
            button1_isDown = true;
            Button1_OnDown?.Invoke();
        }
        
        if (callbackContext.canceled)
        {
            if(debugMessages) Debug.Log($"Button 1 cancelled");
            button1_isDown = false;
            Button1_OnUp?.Invoke();
        }
    }
    
    public void OnButton2(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if(debugMessages) Debug.Log($"Button 2 started");
            button2_isDown = true;
            Button2_OnDown?.Invoke();
        }
        
        if (callbackContext.canceled)
        {
            if(debugMessages) Debug.Log($"Button 2 cancelled");
            button2_isDown = false;
            Button2_OnUp?.Invoke();
        }
    }
    
    public void OnButton3(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if(debugMessages) Debug.Log($"Button 3 started");
            button3_isDown = true;
            Button3_OnDown?.Invoke();
        }
        
        if (callbackContext.canceled)
        {
            if(debugMessages) Debug.Log($"Button 3 cancelled");
            button3_isDown = false;
            Button3_OnUp?.Invoke();
        }
    }
    
    public void OnButton4(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if(debugMessages) Debug.Log($"Button 4 started");
            button4_isDown = true;
            Button4_OnDown?.Invoke();
        }
        
        if (callbackContext.canceled)
        {
            if(debugMessages) Debug.Log($"Button 4 cancelled");
            button4_isDown = false;
            Button4_OnUp?.Invoke();
        }
    }

    #endregion

    #region Button Action Subscription

    private void SubscribeButtonsToActions(bool enable)
    {
        switch (jumpButton)
        {
            case 1:
                Button1_OnDown += OnJumpDown;
                Button1_OnUp += OnJumpUp;
                break;
            case 2:
                Button2_OnDown += OnJumpDown;
                Button2_OnUp += OnJumpUp;
                break;
            case 3:
                Button3_OnDown += OnJumpDown;
                Button3_OnUp += OnJumpUp;
                break;
            case 4:
                Button4_OnDown += OnJumpDown;
                Button4_OnUp += OnJumpUp;
                break;
        }
    }

    #endregion

    
}
