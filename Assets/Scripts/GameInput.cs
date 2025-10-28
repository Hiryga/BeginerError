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

    private PlayerInputActions inputActions;
    
    private void Awake()
    {
        Instance = this;
        inputActions = new PlayerInputActions();
        inputActions.Pause.Pause.performed += ctx => PausePressed();
        inputActions.Player.Enable(); // <--- ќЅя«ј“≈Ћ№Ќќ!
        inputActions.Pause.Enable();  // <--- дл€ карты Pause (если есть Esc)
        inputActions.Combat.Enable();

    }

    private void PausePressed()
    {
        Debug.Log("ESC pressed!"); // ƒобавь дл€ отладки
        OnPause?.Invoke();
    }


    public void DisablePlayerInput()
    {
        inputActions.Player.Disable();
    }

    public void EnablePlayerInput()
    {
        inputActions.Player.Enable();
    }


    public Vector2 GetMovementVector() {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }

    public Vector2 GetAttackingVector()
    {
        Vector2 inputAtkVector = inputActions.Combat.Attack.ReadValue<Vector2>();

        return inputAtkVector;
    }

    public Vector3 GetMousePosition() {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        return mousePos;
    }

}

