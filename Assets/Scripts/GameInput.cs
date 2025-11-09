using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event Action OnPause;
    public event Action OnAttack;
    public event Action OnBowAttack;
    public event Action OnSelectSword;
    public event Action OnSelectBow;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        Instance = this;
        inputActions = new PlayerInputActions();

        // Pause
        inputActions.Pause.Pause.performed += ctx => PausePressed();

        // Enable Action Maps
        inputActions.Player.Enable();
        inputActions.Pause.Enable();
        inputActions.Combat.Enable();
        inputActions.Weapons.Enable();

        // Attack/Combat
        inputActions.Combat.Attack.performed += ctx => OnAttack?.Invoke();
        inputActions.Combat.BowAttack.performed += ctx => OnBowAttack?.Invoke();

        // Sword/Bow Switch (если настроено)
        inputActions.Weapons.SelectSword.performed += ctx => OnSelectSword?.Invoke();
        inputActions.Weapons.SelectBow.performed += ctx => OnSelectBow?.Invoke();
    }

    private void PausePressed()
    {
        Debug.Log("ESC pressed!"); // Для отладки
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

    public Vector2 GetMovementVector()
    {
        return inputActions.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetAttackingVector()
    {
        return inputActions.Combat.Attack.ReadValue<Vector2>();
    }

    public Vector3 GetMousePosition()
    {
        return Mouse.current.position.ReadValue();
    }
}
