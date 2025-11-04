using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{

    public static GameInput Instance { get; private set; }

    public event Action OnPause;

    private PlayerInputActions _inputActions;



    private void Awake()
    {
        Instance = this;
        _inputActions = new PlayerInputActions();
        _inputActions.Pause.Pause.performed += ctx => PausePressed();
        _inputActions.Player.Enable(); // <--- ÎÁßÇÀÒÅËÜÍÎ!
        _inputActions.Pause.Enable();  // <--- äëÿ êàðòû Pause (åñëè åñòü Esc)
        _inputActions.Combat.Enable();

    }

    private void PausePressed()
    {
        OnPause?.Invoke();
    }


    public void DisablePlayerInput()
    {
        _inputActions.Player.Disable();
    }

    public void EnablePlayerInput()
    {
        _inputActions.Player.Enable();
    }


    public Vector2 GetMovementVector() {
        Vector2 inputVector = _inputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }

    public Vector2 GetAttackingVector()
    {
        Vector2 inputAtkVector = _inputActions.Combat.Attack.ReadValue<Vector2>();

        return inputAtkVector;
    }

    public Vector3 GetMousePosition() {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        return mousePos;
    }

}

