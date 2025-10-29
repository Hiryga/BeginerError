using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Sword _sword;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private const string IS_RUNNING = "IsRunning";

    private void Awake() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    private void Update() {
        if (PauseMenu.IsPaused) return;

        animator.SetBool(IS_RUNNING, Player.Instance.IsRunning());
        AdjustPlayerFacingDirection();
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }

    public void TriggerAttackAnimationTurnOn()
    {
        _sword.AttackColliderTurnOn();
    }

    public void TriggerAttackAnimationTurnOff()
    {
        _sword.AttackColliderTurnOff();
    }

    private void AdjustPlayerFacingDirection() {
        Vector3 mousePos = GameInput.Instance.GetMousePosition();
        Vector3 playerPosition = Player.Instance.GetPlayerScreenPosition();

        bool shouldFlip = mousePos.x < playerPosition.x;
        spriteRenderer.flipX = shouldFlip;

        // ≈сли у меча нет спрайта, просто зеркалим его localScale.x
        if (_sword != null)
        {
            Vector3 localScale = _sword.transform.localScale;
            localScale.x = shouldFlip ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
            _sword.transform.localScale = localScale;
        }
    }
}
