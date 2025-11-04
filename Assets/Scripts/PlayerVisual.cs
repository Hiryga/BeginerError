using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Sword _sword;
    private Animator _animator;
    private SpriteRenderer spriteRenderer;
    private const string IS_RUNNING = "IsRunning";
    private const string IS_DEAD = "IsDie";
    private const string ATTACK = "Attack";

    private bool isDead = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
    }

    private void Player_OnPlayerDeath(object sender, System.EventArgs e)
    {
        isDead = true;
        _animator.SetBool(IS_DEAD, true);
    }

    private void Update()
    {
        if (PauseMenu.IsPaused || isDead) return;

        _animator.SetBool(IS_RUNNING, Player.Instance.IsRunning());
        AdjustPlayerFacingDirection();

        if (Player.Instance.IsAlive())
            AdjustPlayerFacingDirection();

        if (Input.GetMouseButtonDown(0))
        {
            _animator.SetTrigger(ATTACK);
        }
    }

    public void TriggerAttackAnimationTurnOn()
    {
        if (isDead) return;
        _sword.AttackColliderTurnOn();
    }

    public void TriggerAttackAnimationTurnOff()
    {
        if (isDead) return;
        _sword.AttackColliderTurnOff();
    }

    private void AdjustPlayerFacingDirection()
    {
        if (isDead) return;

        Vector3 mousePos = GameInput.Instance.GetMousePosition();
        Vector3 playerPosition = Player.Instance.GetPlayerScreenPosition();
        bool shouldFlip = mousePos.x < playerPosition.x;
        spriteRenderer.flipX = shouldFlip;

        if (_sword != null)
        {
            Vector3 localScale = _sword.transform.localScale;
            localScale.x = shouldFlip ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
            _sword.transform.localScale = localScale;
        }
    }
}