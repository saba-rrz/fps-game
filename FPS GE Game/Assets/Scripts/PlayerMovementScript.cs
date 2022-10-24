using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    private CharacterController _controller;
    
    [SerializeField] private float runSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private int jumpCharges;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravity;
    [SerializeField] private float normalGravity;
    [SerializeField] private float airSpeed;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;

    private Vector3 _move;
    private Vector3 _input;
    private Vector3 _yVelocity;
    private Vector3 _crouchingCenter = new Vector3(0, 0.65f, 0);
    private Vector3 _standingCenter = new Vector3(0, 0, 0);

    
    private float _speed;
    private float _startHeight;
    private const float CrouchHeight = 0.65f;
    private bool _isGrounded;
    private bool _isCrouching;
    private bool _isSprinting;
    

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _startHeight = transform.localScale.y;
    }

    void HandleInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        
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

    private void Update()
    {
        HandleInput();
        GroundedMovement();
        if (_isGrounded)
        {
            GroundedMovement();
        }
        else
        {
            AirMovement();
        }
        CheckGround();
        _controller.Move(_move * Time.deltaTime);
        ApplyGravity();
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
        gravity = normalGravity;
        _yVelocity.y += gravity * Time.deltaTime;
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
    }

    void ExitCrouch()
    {
        _controller.height = (_startHeight * 2);
        _controller.center = _standingCenter;
        transform.localScale = new Vector3(transform.localScale.x, _startHeight, transform.localScale.z);
        _isCrouching = false;
    }

    void Sprinting()
    {
        
    }
}

