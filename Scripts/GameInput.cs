using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    public event EventHandler OnJumpAction;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();

        playerInputActions.Player.Jump.performed += Jump_performed;
    }



    private void Jump_performed(InputAction.CallbackContext obj) {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVector2() {
        Vector2 movementVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        return movementVector;
    }
}
