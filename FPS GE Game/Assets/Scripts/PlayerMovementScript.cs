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

    
    private float _speed;
    private bool _isGrounded;
    private bool _isCrouching;
    private bool _isSprinting;
    

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void HandleInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        
        _input = transform.TransformDirection(_input);
        _input = Vector3.ClampMagnitude(_input, 1f);

        if (Input.GetKeyUp(KeyCode.Space) && jumpCharges > 0)
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
        _speed = _isSprinting ? sprintSpeed : _isCrouching ? crouchSpeed : runSpeed;
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
        
    }

    void Sprinting()
    {
        
    }
}

