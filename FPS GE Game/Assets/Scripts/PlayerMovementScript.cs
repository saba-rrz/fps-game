using UnityEngine;


public class PlayerMovementScript : MonoBehaviour
{
    #region Variables & Numbers

    #region Components

    //Components
    [Header("Components")] public CharacterController controller; //Unity Character Controller
    public Transform groundCheck; //Transform to get the GroundCheck object
    public Camera playerCamera; //Player Camera component

    #endregion

    #region Layer Masks

    //Layer Masks
    [Header("Layer Masks")] [SerializeField]
    private LayerMask groundMask; //LayerMask to detects objects that have the ground layer

    [SerializeField] private LayerMask wallMask; //LayerMask to detects objects that have the wall layer

    #endregion

    #region Vectors

    //Vectors
    private readonly Vector3 _crouchingCenter = new Vector3(0, 0.5f, 0);
    private readonly Vector3 _standingCenter = new Vector3(0, 0, 0);
    private Vector3 _wallNormal;
    private Vector3 _lastWall;
    private Vector3 _move;
    private Vector3 _input;
    private Vector3 _yVelocity;
    private Vector3 _forwardDirection;

    #endregion

    #region Raycasts

    private RaycastHit _leftWallHit;
    private RaycastHit _rightWallHit;
    private RaycastHit _wallHit;

    #endregion

    #region Crouch Variables

    private float _startHeight;
    public float crouchHeight = 0.5f;

    #endregion

    #region Speed Settings

    //Speed Settings 
    [Header("Speed Settings")] [SerializeField]
    private float speed;

    [SerializeField] private float runSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float airSpeed;
    [SerializeField] private float climbSpeed;

    #endregion

    #region Gravity Settings
    
    [Header("Gravity Settings")]
    [SerializeField] private float normalGravity;
    [SerializeField] private float wallRunGravity;
    private float _gravity;

    #endregion

    #region Jump Settings
    
    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private AudioSource jumpSound;
    private int _jumpCharges;

    #endregion

    #region Slide Settings
    
    [Header("Slide Settings")]
    [SerializeField] private float slideSpeedIncrease;
    [SerializeField] private float slideSpeedDecrease;
    [SerializeField] private float slideTimer;
    [SerializeField] private float maxSlideTimer;

    #endregion

    #region Bools

    private bool _isSprinting;
    private bool _isCrouching;
    private bool _isSliding;
    private bool _isWallRunning;
    private bool _isGrounded;
    private bool _onLeftWall;
    private bool _onRightWall;
    private bool _hasWallRun;
    private bool _isClimbing;
    private bool _hasClimbed;
    private bool _canClimb;
    private bool _isWallJumping;

    #endregion

    #region Wall Run Settings
    
    [Header("Wall Running Settings")]
    [SerializeField] private float wallRunSpeedIncrease;
    [SerializeField] private float wallRunSpeedDecrease;
    [SerializeField] private float maxWallJumpTimer;
    private float _wallJumpTimer;
    
    #endregion

    #region Climbing Settings
    
    [Header("Climbing Settings")]
    [SerializeField] private float climbTimer;
    public float maxClimbTimer;

    #endregion

    #region Camera Settings
    [Header("Camera Settings")]
    [SerializeField] private float specialFOV;
    [SerializeField] private float cameraChangeTime;
    [SerializeField] private float wallRunTilt;
    [SerializeField] public float tilt;
    private float _normalFOV;

    #endregion
    
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
            slideTimer -= 1f * Time.deltaTime; // Timer for sliding time
            if (slideTimer < 0) // If the timer runs out
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
            climbTimer -= 1f * Time.deltaTime;
            if (climbTimer < 0)
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

        if (!_isWallRunning)
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
            jumpSound.Play();
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
            climbTimer = maxClimbTimer;
        }
    }

    void CheckWallRun()
    {
        var playerTransform = transform;
        var position = playerTransform.position;
        var right = playerTransform.right;
        _onRightWall = Physics.Raycast(position, right, out _rightWallHit, 0.7f, wallMask);
        _onLeftWall = Physics.Raycast(position, -right, out _leftWallHit, 0.7f, wallMask);

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
        var playerTransform = transform;
        _canClimb = Physics.Raycast(playerTransform.position, playerTransform.forward, out _wallHit, 0.7f, wallMask);
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
        controller.height = crouchHeight;
        controller.center = _crouchingCenter;
        var playerTransform = transform;
        var localScale = playerTransform.localScale;
        
        localScale = new Vector3(localScale.x, crouchHeight, localScale.z);
        playerTransform.localScale = localScale;
        if (speed >  (runSpeed * 0.95))
        {
            _isSliding = true;
            _forwardDirection = transform.forward;
            if (_isGrounded)
            {
                IncreaseSpeed(slideSpeedIncrease);
            }

            slideTimer = maxSlideTimer;
        }
        _isCrouching = true;
    }

    void ExitCrouch()
    {
        controller.height = (_startHeight * 2);
        controller.center = _standingCenter;
        var playerTransform = transform;
        var localScale = playerTransform.localScale;
        
        localScale = new Vector3(localScale.x, _startHeight, localScale.z);
        playerTransform.localScale = localScale;
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
            //IncreaseSpeed(wallRunSpeedIncrease);
            ExitWallRun();
        }
        _hasClimbed = false;
        climbTimer = maxClimbTimer;
        _yVelocity.y = Mathf.Sqrt(jumpHeight * -2f * normalGravity);
    }

    void ApplyGravity() 
    {
        _gravity = _isWallRunning ? wallRunGravity : _isClimbing ? 0f : normalGravity;
        if (_yVelocity.y > _gravity)
        {
            _yVelocity.y += _gravity * Time.deltaTime; 
        }
        controller.Move(_yVelocity * Time.deltaTime);
    }
}

