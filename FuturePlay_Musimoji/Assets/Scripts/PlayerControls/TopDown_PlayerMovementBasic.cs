using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TopDown_PlayerMovementBasic : MonoBehaviour
{
    public bool debugMessages = false;

    [SerializeField] private CircleCollider2D playerCollider;

    [SerializeField] private LayerMask collisionLayers;
    
    [Header("Settings")]
    public float moveSpeed = 5;

    public bool lookatMouse = false, rotateLookActive = true;

    private Vector3 move;
    [SerializeField] private Vector2 moveDir;
    private float aim;
    [SerializeField] private Vector2 aimDir;
    [SerializeField] private Vector2 mousePos;

    private bool anyKey_isDown, button1_isDown, button2_isDown, button3_isDown, button4_isDown;

    public int PlayerID { get; private set; }

    public void SetID(int playerID)
    {
        PlayerID = playerID;
    }
    
    #region Gameplay

    private void Update()
    {
        Move();
        Aim();
    }

    private void Move()
    {
        move = new Vector3(moveDir.x, moveDir.y, 0.0f).normalized * (moveSpeed * Time.deltaTime);
        if(MoveBlocked(move)) return;
        transform.Translate(move, Space.World);
    }

    private bool MoveBlocked(Vector3 moveTo)
    {
        var collisions = new Collider2D[32];
        
        Physics2D.OverlapCircleNonAlloc(transform.position + moveTo, playerCollider.radius, collisions, collisionLayers);

        var otherCollisions = new List<Collider2D>();

        foreach (var c in collisions)
        {
            if (c == null) continue;
            
            // if(debugMessages) Debug.Log($"PlayerMovementBasic.MoveBlocked {c.name}");
            
            if(c.gameObject!=this.gameObject) otherCollisions.Add(c);
        }

        return otherCollisions.Count > 0;
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

    //Unity Event
    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        // if(debugMessages) Debug.LogWarning($"Device: {callbackContext.control.device.description.interfaceName}");
        
        /*if (callbackContext.control.device.description.interfaceName == "HID")
        {
            //Pimoroni values -0.01, 0.71, 0.01
            var inputValue = callbackContext.ReadValue<Vector2>();
            
            if(debugMessages) Debug.LogWarning($"Joystick HID: input({inputValue})");
            
            moveDir = Vector2.zero;

            if (inputValue.x > 0 && inputValue.x < 0.1) moveDir.x = 1;
            if (inputValue.x < 0 && inputValue.x > -0.1) moveDir.x = -1;
            // if (inputValue.x > 0.1 && inputValue.x < 1) moveDir.x = 0;
            // if (inputValue.x < -0.1 && inputValue.x > -1) moveDir.x = 0;
            
            if (inputValue.y > 0 && inputValue.y < 0.1) moveDir.y = 1;
            if (inputValue.y < 0 && inputValue.y > -0.1) moveDir.y = -1;
            // if (inputValue.y > 0.1 && inputValue.y < 1) moveDir.y = 0;
            // if (inputValue.y < -0.1 && inputValue.y > -1) moveDir.y = 0;
            
            
            if(debugMessages) Debug.LogWarning($"Joystick HID: MoveDir({moveDir})");
            
            return;
        }*/
        
        //Keyboard values 1,0,-1
        //Gamepad values -1 <=> 1
        moveDir = callbackContext.ReadValue<Vector2>();
    }
    
    //Unity Event
    public void OnAim(InputAction.CallbackContext callbackContext)
    {
        if (!rotateLookActive) return;
        
        var position = (Vector2) transform.position;

        var direction = callbackContext.ReadValue<Vector2>();
        
        Debug.DrawLine(position, position + direction, Color.magenta);
        
        aimDir = direction;
    }
    
    //Unity Event
    public void OnMousePosition(InputAction.CallbackContext callbackContext)
    {
        if (!lookatMouse) return;
        
        var position = transform.position;

        mousePos = Camera.main.ScreenToWorldPoint(callbackContext.ReadValue<Vector2>());

        Debug.DrawLine(position, mousePos, Color.magenta);
        
        var angle = Mathf.Atan2(position.y-mousePos.y, position.x-mousePos.x) * Mathf.Rad2Deg + 90;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
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
        }
        
        if (callbackContext.canceled)
        {
            if(debugMessages) Debug.Log($"Button 1 cancelled");
            button1_isDown = false;
        }
    }
    
    public void OnButton2(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if(debugMessages) Debug.Log($"Button 2 started");
            button2_isDown = true;
        }
        
        if (callbackContext.canceled)
        {
            if(debugMessages) Debug.Log($"Button 2 cancelled");
            button2_isDown = false;
        }
    }
    
    public void OnButton3(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if(debugMessages) Debug.Log($"Button 3 started");
            button3_isDown = true;
        }
        
        if (callbackContext.canceled)
        {
            if(debugMessages) Debug.Log($"Button 3 cancelled");
            button3_isDown = false;
        }
    }
    
    public void OnButton4(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if(debugMessages) Debug.Log($"Button 4 started");
            button4_isDown = true;
        }
        
        if (callbackContext.canceled)
        {
            if(debugMessages) Debug.Log($"Button 4 cancelled");
            button4_isDown = false;
        }
    }

    #endregion

    
}
