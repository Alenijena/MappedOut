using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

class PlayerStatsSave {
    public float moveSpeed;
    public float sideMoveSpeed;
    public float friction;
    public float jumpForce;
    public float jumpingTimer;
    public float jumpDeacceleration;
    public float gravityForce;
    public float playerHeight;
}

public class Player : MonoBehaviour
{   
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float backWardsMoveSpeed = 0.7f;
    [SerializeField] private float sideMoveSpeed = 2f;
    [SerializeField] private float friction = .7f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpingTimer = 20f;
    [SerializeField] private float jumpDeacceleration = 0.3f;
    [SerializeField] private float gravityForce = 50f;
    [SerializeField] private float cayoteTime = 15f;
    [SerializeField, Range(0, 1)] private float playerScaleRatio;
    [SerializeField] private float playerHeight = 1f;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Rigidbody rigidBody;

    private Vector3 currentVelocity;
    private float gravityVelocity;
    private float jumpingCounter;
    private float jumpingVelocity;
    private float cayoteCounter;

    private PlayerStatsSave playerStatsSave = new PlayerStatsSave();

    private void Awake() {
        gameInput.OnJumpAction += GameInput_OnJumpAction;

        playerStatsSave.moveSpeed = moveSpeed;
        playerStatsSave.sideMoveSpeed = sideMoveSpeed;
        playerStatsSave.friction = friction;
        playerStatsSave.jumpForce = jumpForce;
        playerStatsSave.jumpingTimer = jumpingTimer;
        playerStatsSave.jumpDeacceleration = jumpDeacceleration;
        playerStatsSave.gravityForce = gravityForce;
        playerStatsSave.playerHeight = playerHeight;
    }

    public float GetScaleRatio() {
        return playerScaleRatio;
    }

    public bool IsGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight);
    }

    public bool UnaccurateIsgrounded() {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight + 0.5f);
    }

    public Vector2 GetVelocityXZ() {
        return new Vector2(rigidBody.velocity.x, rigidBody.velocity.z);
    }

    private void GameInput_OnJumpAction(object sender, EventArgs e) {
        if (cayoteCounter <= 0) {return;}
            jumpingCounter = jumpingTimer;
    }

    private void HandleGravity() {
        if (!IsGrounded()) {
            gravityVelocity += gravityForce * Time.deltaTime;
            cayoteCounter -= 1 * Time.deltaTime;
        } else {
            gravityVelocity = 0;
            cayoteCounter = cayoteTime;
        }

        transform.position -= new Vector3(0, gravityVelocity, 0);
    }
    
    private void HandleJumping() {
        if (jumpingCounter > 0) {
            jumpingVelocity += jumpForce * Time.deltaTime;
            jumpingCounter -= 1 * Time.deltaTime;
        } else {
            jumpingVelocity = math.lerp(jumpingVelocity, 0, jumpDeacceleration);
        }

        bool canMove = !Physics.Raycast(transform.position, Vector3.up, playerHeight);

        if (canMove) {
            transform.position += new Vector3(0, jumpingVelocity, 0);
        } else {
            jumpingCounter = 0;
        }
    }

    private Vector3 GetLookDirection() {
        return new Vector3(mainCamera.forward.x, transform.forward.y, mainCamera.forward.z);
    }

    private Vector3 GetSideDirection() {
        return new Vector3(mainCamera.right.x, transform.forward.y, mainCamera.right.z);
    }

    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVector2();

        float backWardsMultiplier = inputVector.y < 0 ? backWardsMoveSpeed : 1;

        Vector3 moveDir = GetLookDirection() * inputVector.y * backWardsMultiplier + GetSideDirection() * inputVector.x * sideMoveSpeed * backWardsMultiplier;
        
        if (moveDir != Vector3.zero) {
            currentVelocity = (moveDir * moveSpeed * Time.deltaTime) + new Vector3(0, rigidBody.velocity.y, 0);
        } else {
            if (IsGrounded()) {
                currentVelocity = Vector3.Lerp(currentVelocity, new Vector3(0, rigidBody.velocity.y, 0), friction);
            } else {
                currentVelocity = new Vector3(0, rigidBody.velocity.y, 0);
            }
        }

        rigidBody.velocity = currentVelocity;
    }

    public bool IsWalking() {
        return math.abs(rigidBody.velocity.x) > 0.1 || math.abs(rigidBody.velocity.z) > 0.1;
    }

    private void HandleScaling() {
        float scaleSpeed = 0.5f;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * playerScaleRatio, scaleSpeed);
        moveSpeed = playerStatsSave.moveSpeed * playerScaleRatio;
        jumpingTimer = playerStatsSave.jumpingTimer * playerScaleRatio;
        gravityForce = playerStatsSave.gravityForce * playerScaleRatio;
        playerHeight = playerStatsSave.playerHeight * playerScaleRatio;
    }

    private void FixedUpdate() {
        HandleMovement();
        HandleGravity();
        HandleJumping();
        HandleScaling();
    }
}
