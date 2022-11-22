using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovementScript : MonoBehaviour
{
    #region Variables & Numbers

    #region Components

    //Components
    public CharacterController controller; //Unity Character Controller
    public Transform groundCheck; //Transform to get the GroundCheck object

    #endregion
    public LayerMask groundMask;
    public LayerMask wallMask;
 
    Vector3 _move;
    Vector3 _input;
    Vector3 _yVelocity;
    Vector3 _forwardDirection;

    [FormerlySerializedAs("_speed")] [SerializeField] float speed;

    public float runSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float airSpeed;
    public float climbSpeed;

    float _gravity;
    public float normalGravity;
    public float wallRunGravity;
    public float jumpHeight;

    public float slideSpeedIncrease;
    public float wallRunSpeedIncrease;
    public float slideSpeedDecrease;
    public float wallRunSpeedDecrease;

    int _jumpCharges;

    bool _isSprinting;
    bool _isCrouching;
    bool _isSliding;
    bool _isWallRunning;
    bool _isGrounded;

    float _startHeight;
    float _crouchHeight = 0.5f;
    float _slideTimer;
    public float maxSlideTimer;
    Vector3 _crouchingCenter = new Vector3(0, 0.5f, 0);
    Vector3 _standingCenter = new Vector3(0, 0, 0);

    bool _onLeftWall;
    bool _onRightWall;
    bool _hasWallRun = false;
    private RaycastHit _leftWallHit;
    private RaycastHit _rightWallHit;
    Vector3 _wallNormal;
    Vector3 _lastWall;

    bool _isClimbing;
    bool _hasClimbed;
    bool _canClimb;
    private RaycastHit _wallHit;

    float _climbTimer;
    public float maxClimbTimer;

    bool _isWallJumping;
    float _wallJumpTimer;
    public float maxWallJumpTimer;

    public Camera playerCamera;
    float _normalFOV;
    public float specialFOV;
    public float cameraChangeTime;
    public float wallRunTilt;
    public float tilt;
    #endregion



    // Start function
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        _startHeight = transform.localScale.y;
        _jumpCharges = 2;
        _normalFOV = playerCamera.fieldOfView;
    }

    void IncreaseSpeed(float speedIncrease)
    {
        speed += speedIncrease;
    }

    void DecreaseSpeed(float speedDecrease)
    {
        speed += speedDecrease * Time.deltaTime;
    }
    
    //Update functions
    private void Update()
    {
        HandleInput(); // Calls the input function
        CheckWallRun(); // Constantly checks if player is wall running
        CheckClimbing(); // Constantly checks if player is climbing
        if (_isGrounded && !_isSliding) //Checks if on the ground and not sliding; uses the ground movement
        {
            GroundedMovement(); //Calls ground movement function
        }
        else if (!_isGrounded && !_isWallRunning && !_isClimbing) //If not on the ground use air movement
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
        else if (_isWallRunning)
        {
            WallRunningMovement();
            DecreaseSpeed(wallRunSpeedDecrease);
        }
        else if (_isClimbing)
        {
            ClimbMovement();
            _climbTimer -= 1f * Time.deltaTime;
            if (_climbTimer < 0)
            {
                _isClimbing = false;
                _hasClimbed = true;
            }
        }
        
        controller.Move(_move * Time.deltaTime); //Character movement independent of frame-rate
        ApplyGravity(); // Applies gravity using the Gravity function
        CameraEffects();
    }
    
    private void FixedUpdate()
    {
        CheckGround(); // Constantly checks if player is grounded
    }
    
    //Camera Effects
    void CameraEffects()
    {
        float fov = _isWallRunning ? specialFOV : _isSliding ? specialFOV : _normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView,fov, cameraChangeTime * Time.deltaTime);

        if (_isWallRunning)
        {
            if (_onRightWall)
            {
                tilt = Mathf.Lerp(tilt, wallRunTilt, cameraChangeTime * Time.deltaTime);
            }
            if (_onLeftWall)
            {
                tilt = Mathf.Lerp(tilt, -wallRunTilt, cameraChangeTime * Time.deltaTime);
            }
        }

        if (!_isWallJumping)
        {
            tilt = Mathf.Lerp(tilt, 0f, cameraChangeTime * Time.deltaTime);
        }

    }

    //Function for receiving inputs
    void HandleInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")); // Gets the input for Horizontal and Vertical movement

        if (!_isWallRunning)
        {
            _input = transform.TransformDirection(_input);
            _input = Vector3.ClampMagnitude(_input, 1f);
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
        
        if (Input.GetKeyDown(KeyCode.Space) && _jumpCharges > 0)
        {
            Jump();
        }
    }
    
    void CheckGround()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, 0.3f, groundMask);
        if (_isGrounded)
        {
            _jumpCharges = 1;
            _hasWallRun = false;
            _hasClimbed = false;
            _climbTimer = maxClimbTimer;
        }
    }

    void CheckWallRun()
    {
        _onRightWall = Physics.Raycast(transform.position, transform.right, out _rightWallHit, 0.7f, wallMask);
        _onLeftWall = Physics.Raycast(transform.position, -transform.right, out _leftWallHit, 0.7f, wallMask);

        if ((_onRightWall || _onLeftWall) && !_isWallRunning)
        {
            TestWallRun();
        }
        else if (!_onRightWall && !_onLeftWall && _isWallRunning)
        {
            ExitWallRun();
        }
    }
    
    void CheckClimbing()
    {
        _canClimb = Physics.Raycast(transform.position, transform.forward, out _wallHit, 0.7f, wallMask);
        float wallAngle = Vector3.Angle(-_wallHit.normal, transform.forward);
        if (wallAngle < 15 && _canClimb && !_hasClimbed)
        {
            _isClimbing = true;
        }
        else
        {
            _isClimbing = false;
        }
    }
    
    void GroundedMovement()
    {
        speed = _isSprinting ? sprintSpeed : _isCrouching ? crouchSpeed : runSpeed;
        if (_input.x != 0)
        {
            _move.x += _input.x * speed;
        }
        else
        {
            _move.x = 0;
        }

        if (_input.z != 0)
        {
            _move.z += _input.z * speed;
        }
        else
        {
            _move.z = 0;
        }
        
        _move = Vector3.ClampMagnitude(_move, speed);
    }

    void AirMovement()
    {
        _move.x += _input.x * airSpeed;
        _move.z += _input.z * airSpeed;
        if (_isWallJumping)
        {
            _move += _forwardDirection * airSpeed;
            _wallJumpTimer -= 1f * Time.deltaTime;
            if (_wallJumpTimer <= 0)
            {
                _isWallJumping = false;
            }
        }

        _move = Vector3.ClampMagnitude(_move, speed);
    }

    void SlideMovement()
    {
        _move += _forwardDirection;
        _move = Vector3.ClampMagnitude(_move, speed);
    }

    void WallRunningMovement()
    {
        if (_input.z > (_forwardDirection.z - 10f) && _input.z < (_forwardDirection.z + 10f))
        {
            _move += _forwardDirection;
        }
        else if (_input.z < (_forwardDirection.z - 10f) && _input.z > _forwardDirection.z + 10f)
        {
            _move.x = 0f;
            _move.z = 0f;
            ExitWallRun();
        }
        
        _move.x += _input.x * airSpeed;

        _move = Vector3.ClampMagnitude(_move, speed);
    }
    
    void ClimbMovement()
    {
        _forwardDirection = Vector3.up;
        _move.x += _input.x * airSpeed;
        _move.z += _input.z * airSpeed;

        _yVelocity += _forwardDirection;
        speed = climbSpeed;

        _move = Vector3.ClampMagnitude( _move, speed );
        _yVelocity = Vector3.ClampMagnitude(_yVelocity, speed );
    }
    
    void Crouch()
    {
        controller.height = _crouchHeight;
        controller.center = _crouchingCenter;
        transform.localScale = new Vector3(transform.localScale.x, _crouchHeight, transform.localScale.z);
        if (speed > runSpeed)
        {
            _isSliding = true;
            _forwardDirection = transform.forward;
            if (_isGrounded)
            {
                IncreaseSpeed(slideSpeedIncrease);
            }

            _slideTimer = maxSlideTimer;
        }
        _isCrouching = true;
    }

    void ExitCrouch()
    {
        controller.height = (_startHeight * 2);
        controller.center = _standingCenter;
        transform.localScale = new Vector3(transform.localScale.x, _startHeight, transform.localScale.z);
        _isCrouching = false;
        _isSliding = false;
    }
    
    void TestWallRun()
    {
        _wallNormal = _onRightWall ? _rightWallHit.normal : _leftWallHit.normal;
        if (_hasWallRun)
        {
            float wallAngle = Vector3.Angle(_wallNormal, _lastWall);
            if (wallAngle > 15)
            {
                WallRun();
            }
        }
        else
        {
            _hasWallRun = true;
            WallRun();
        }
    }
    
    void WallRun()
    {
        _isWallRunning = true;
        _jumpCharges = 1;
        _yVelocity = new Vector3(0f, 0f, 0f);
        
        _forwardDirection = Vector3.Cross(_wallNormal, Vector3.up);

        if (Vector3.Dot(_forwardDirection, transform.forward) < 0)
        {
            _forwardDirection = -_forwardDirection;
        }
    }

    void ExitWallRun()
    {
        _isWallRunning = false;
        _lastWall = _wallNormal;
        _forwardDirection = _wallNormal;
        IncreaseSpeed(wallRunSpeedIncrease);
        _isWallJumping = true;
        _wallJumpTimer = maxWallJumpTimer;
    }
    
    void Jump()
    {
        if (!_isGrounded && !_isWallRunning)
        {
            _jumpCharges -= 1;
        }
        else if (_isWallRunning)
        {
            ExitWallRun();
            IncreaseSpeed(wallRunSpeedIncrease);
        }
        _hasClimbed = false;
        _climbTimer = maxClimbTimer;
        _yVelocity.y = Mathf.Sqrt(jumpHeight * -2f * normalGravity);
    }

    void ApplyGravity() 
    {
        _gravity = _isWallRunning ? wallRunGravity : _isClimbing ? 0f : normalGravity;
        _yVelocity.y += _gravity * Time.deltaTime;
        controller.Move(_yVelocity * Time.deltaTime);
    }
}

