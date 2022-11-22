using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovementScript : MonoBehaviour
{
    private static CharacterController _controller; //Unity Character Controller
    
    
    //Settings for scripts - Can be changed in Inspector
    [Header("Speed Settings")]
    [SerializeField] private float runSpeed; //Normal Speed
    [SerializeField] private float sprintSpeed; //Sprinting Speed
    [SerializeField] private float crouchSpeed; //Crouching Speed
    [SerializeField] private float airSpeed; //In Air Speed
    
    [Header("Jump Settings")]
    [SerializeField] private int jumpCharges; //Amount of jumps left
    [SerializeField] private float jumpHeight; // Height  of the jumps
    
    [Header("Gravity Settings")]
    [SerializeField] private float normalGravity; //Gravity force
    [SerializeField] private float wallRunGravity; //Gravity force while wall running
    
    [Header("Slide Settings")]
    [SerializeField] private float maxSlideTimer; //Maximum time to slide
    [SerializeField] private float slideSpeedIncrease; //When starting to slide increases speed by this amount
    [SerializeField] private float slideSpeedDecrease; //When sliding decrease speed by this amount
    
    //Physics Checks
    public Transform groundCheck; //Transform to get the GroundCheck object
    public LayerMask groundMask; //LayerMask for the ground
    public LayerMask wallMask; //LayerMask for the walls
    public float groundDistance = 0.4f; //Variable to make it look smoother

    //Movement Checks
    private Vector3 _move; //Move vector
    private Vector3 _input; //Input Vector
    private Vector3 _yVelocity; //Velocity Vector
    private Vector3 _forwardDirection; //Forward Direction Vector
    private Vector3 _crouchingCenter = new Vector3(0, 0.65f, 0); //Vector for the center of the character while crouching
    private Vector3 _standingCenter = new Vector3(0, 0, 0); //Vector for the center of the character while standing
    
   

    //Constant Variables
    private float _gravity; // Gravity constant 
    private float _speed; // Speed constant
    private float _startHeight = 1f; // Starting Height of the player
    private const float CrouchHeight = 0.65f; // Crouch Height of the player
    private float _slideTimer; //Timer for sliding
   
    //Constant Bools
    private bool _isGrounded; //True : On the Ground \\ False : In the air
    private bool _isCrouching; //True: Crouching \\ False : Not Crouching
    private bool _isSprinting; //True : Sprinting \\ False : Not Sprinting
    private bool _isSliding; //True : Sliding \\ False : Not Sliding
    private bool _isWallRunning; //True : On a Wall \\ False : Off a wall



    // Start function
    private void Start()
    {
        _controller = GetComponent<CharacterController>(); //Gets the character controller component on start
        _startHeight = transform.localScale.y; //Sets the player height on start
    }

    void IncreaseSpeed(float speedIncrease)
    {
        _speed += speedIncrease;
    }

    void DecreaseSpeed(float speedDecrease)
    {
        _speed += speedDecrease * Time.deltaTime;
    }

    //Function for receiving inputs
    void HandleInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")); // Gets the input for Horizontal and Vertical movement
        
        _input = transform.TransformDirection(_input); 
        _input = Vector3.ClampMagnitude(_input, 1f);

        if (Input.GetKeyDown(KeyCode.Space) && jumpCharges > 0)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
        
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            ExitCrouch();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && _isGrounded)
        {
            _isSprinting = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isSprinting = false;
        }
    }

    //Update functions
    private void Update()
    {
        HandleInput(); // Calls the input function
        if (_isGrounded && !_isSliding) //Checks if on the ground and not sliding; uses the ground movement
        {
            GroundedMovement(); //Calls ground movement function
        }
        else if (!_isGrounded) //If not on the ground use air movement
        {
            AirMovement(); // Calls air movement function
        }
        else if (_isSliding) //If sliding use sliding movement
        {
            SlideMovement(); // Calls the sliding movement function
            DecreaseSpeed(slideSpeedDecrease); //Calls the speed decrease function
            _slideTimer -= 1f * Time.deltaTime; // Timer for sliding time
            if (_slideTimer < 0) // If the timer runs out
            {
                _isSliding = false; // Sets sliding to false
            }
        }
        CheckGround(); // Constantly checks if player is grounded
        _controller.Move(_move * Time.deltaTime); //Character movement independent of frame-rate
        ApplyGravity(); // Applies gravity using the Gravity function
    }

    void GroundedMovement()
    {
        _speed = _isSprinting ? sprintSpeed : _isCrouching && _isGrounded ? crouchSpeed : runSpeed;
        if (_input.x != 0)
        {
            _move.x += _input.x * _speed;
        }
        else
        {
            _move.x = 0;
        }

        if (_input.z != 0)
        {
            _move.z += _input.z * _speed;
        }
        else
        {
            _move.z = 0;
        }
        
        _move = Vector3.ClampMagnitude(_move, _speed);
    }

    void AirMovement()
    {
        _move.x += _input.x * airSpeed;
        _move.z += _input.z * airSpeed;

        _move = Vector3.ClampMagnitude(_move, _speed);
    }

    void SlideMovement()
    {
        _move += _forwardDirection;
        _move = Vector3.ClampMagnitude(_move, _speed);
    }

    void CheckGround()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (_isGrounded && _yVelocity.y < 0)
        {
            jumpCharges = 2;
            _yVelocity.y = 0.2f;
        } 
    }

    void ApplyGravity() 
    {
        _gravity = normalGravity;
        _yVelocity.y += _gravity * Time.deltaTime;
        _controller.Move(_yVelocity * Time.deltaTime);
    }

    void Jump()
    {
        _yVelocity.y = Mathf.Sqrt(jumpHeight * -2f * normalGravity);
        jumpCharges--;
    }

    void Crouch()
    {
        _controller.height = CrouchHeight;
        _controller.center = _crouchingCenter;
        transform.localScale = new Vector3(transform.localScale.x, CrouchHeight, transform.localScale.z);
        _isCrouching = true;
        if (_speed > runSpeed)
        {
            _isSliding = true;
            _forwardDirection = transform.forward;
            if (_isGrounded)
            {
                IncreaseSpeed(slideSpeedIncrease);
            }

            _slideTimer = maxSlideTimer;
        }
    }

    void ExitCrouch()
    {
        _controller.height = (_startHeight * 2);
        _controller.center = _standingCenter;
        transform.localScale = new Vector3(transform.localScale.x, _startHeight, transform.localScale.z);
        _isCrouching = false;
        _isSliding = false;
    }
}

