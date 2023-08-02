using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlatformerLadders_PlayerMovementBasic : MonoBehaviour
{
    public bool debugMessages = false;

    [SerializeField] private BoxCollider2D playerCollider;

    [SerializeField, Tooltip("Player will collide with colliders on these layers eg. walls, ceiling & floors")] 
    private LayerMask obstacleLayers;

    [SerializeField] 
    private string platformTag = "Platform";

    [SerializeField, Tooltip("Player will be considered 'on a ladder' when in front of colliders on these layers")] 
    private LayerMask ladderLayer;

    [Header("Settings"), Tooltip("Fall faster")]
    public float gravity = 9.8f;
    [Tooltip("Move left & right")]
    public float moveSpeed = 5;
    [Tooltip("Move up & down ladders at this speed")]
    public float ladderSpeed = 3;
    [Tooltip("Vertical velocity while jump button is held")]
    public float jumpPower = 8f;
    [Tooltip("How long jump will remain active while holding button")]
    public float jumpDuration = 0.2f;
    [Tooltip("If true, pressing 'up' on the joystick will trigger 'jump'")]
    public bool jumpWithUpDirection = false;

    public float belowGroundSnapSize = 0.1f;
    
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

    private Vector2 PlayerTopBounds => transform.position + new Vector3(0,playerCollider.size.y,0);

    [SerializeField] private bool isGrounded = false, isOnLadder = false;

    private bool anyKey_isDown, button1_isDown, button2_isDown, button3_isDown, button4_isDown;

    public Action Button1_OnDown, Button2_OnDown, Button3_OnDown, Button4_OnDown;
    public Action Button1_OnUp, Button2_OnUp, Button3_OnUp, Button4_OnUp;

    public UnityEvent OnJump;
    public UnityEvent<Vector2> OnMoveDirection;
    public UnityEvent OnLadderEnter, OnLadderExit;
    public UnityEvent OnLandGround;

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
    
    private void FixedUpdate()
    {
        LadderCheck();
        SetVelocity();
        CheckGrounded();
        CheckMoveCollision();
        Move();
        Aim();
    }

    private void LadderCheck()
    {
        var currentPosition = (Vector2)transform.position + playerCollider.offset;
        
        var results = new Collider2D[1];

        var wasOnLadder = isOnLadder;
        
        isOnLadder = Physics2D.OverlapBoxNonAlloc(currentPosition, playerCollider.size  * 0.9f, 0f, results, ladderLayer) > 0;
        
        switch (wasOnLadder)
        {
            case true when !isOnLadder:
                OnLadderExit?.Invoke();
                break;
            case false when isOnLadder:
                OnLadderEnter?.Invoke();
                break;
        }
    }

    private void SetVelocity()
    {
        velocity.x = moveDir.x * (moveSpeed * Time.deltaTime);

        if (isOnLadder)
        {
            velocity.y = moveDir.y * ladderSpeed * Time.deltaTime;
            isGrounded = false;
            return;
        }
        
        velocity.y -= gravity * Time.deltaTime;

        if (jumpWithUpDirection && !jumpExpended && moveDir.y > 0) OnJumpDown();

        if (jumpWithUpDirection && moveDir.y <= 0) OnJumpUp();

        Jump();
    }

    private void Move()
    {
        transform.Translate(velocity, Space.World);
        OnMoveDirection?.Invoke(velocity);
    }

    private void CheckGrounded()
    {
        if (isOnLadder) return;
        
        if(velocity.y>0) return;

        var wasGrounded = isGrounded;

        // isGrounded = true;
        
        var currentPosition = (Vector2)transform.position + playerCollider.offset;
        
        Debug.DrawLine(currentPosition, currentPosition + velocity, Color.green);

        var nonPlayerCollisionCount = 0;
        var collisions = Physics2D.BoxCastAll(currentPosition, playerCollider.size * transform.lossyScale * 0.9f, 0f, Vector2.down,
            1f, obstacleLayers);

        foreach (var hit in collisions)
        {
            var groundHeight = hit.collider.bounds.max.y;
            
            if (groundHeight < transform.position.y - belowGroundSnapSize) continue;

            if (groundHeight < transform.position.y)
            {
                var newPos = new Vector3(transform.position.x, groundHeight, transform.position.z);
                
                transform.SetPositionAndRotation(newPos, Quaternion.identity);

                continue;
            }

            isGrounded = true;
        
            jumpTimer = 0f;

            if(velocity.y<0) velocity.y = 0f;
            
            if(!jumpRequested) jumpExpended = false;
        }
        
        if(!wasGrounded && isGrounded) OnLandGround?.Invoke();
    }

    private bool CheckMoveCollision()
    {
        var currentPosition = (Vector2)transform.position + playerCollider.offset;
        
        Debug.DrawLine(currentPosition, currentPosition + velocity, Color.green);

        var nonPlayerCollisionCount = 0;
        var collisions = Physics2D.BoxCastAll(currentPosition, 
            playerCollider.size * transform.lossyScale * 0.9f, 0f, velocity,
            Vector2.Distance(currentPosition, currentPosition + velocity), obstacleLayers);

        foreach (var col in collisions)
        {
            if(col.transform.gameObject==this.gameObject) continue;
            
            nonPlayerCollisionCount++;

            // var playerColliderTop = transform.position.y + (playerCollider.bounds.max.y * 0.9f);
            
            //Collision above & moving up
            if (!col.transform.gameObject.CompareTag(platformTag) && (col.point.y > transform.position.y) &&
                velocity.y > 0)
                continue;
            
            //Collision left & moving left
            if ((col.point.x < transform.position.x) && velocity.x < 0) velocity.x = 0;
            
            //Collision right & moving right
            if ((col.point.x > transform.position.x) && velocity.x > 0) velocity.x = 0;
        }

        return nonPlayerCollisionCount > 0;
    }

    private void Jump()
    {
        if (!jumpRequested || jumpExpended || isOnLadder) return;
        
        OnJump?.Invoke();

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
