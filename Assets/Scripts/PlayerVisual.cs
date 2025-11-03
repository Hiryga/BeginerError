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
    private const string IS_DEAD = "IsDead";

    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Player.Instance.GetComponent<PlayerHealth>().OnDeath += HandleDeath;
    }

    private void Update()
    {
        if (PauseMenu.IsPaused || isDead) return;

        animator.SetBool(IS_RUNNING, Player.Instance.IsRunning());
        AdjustPlayerFacingDirection();

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }

    private void HandleDeath(object sender, System.EventArgs e)
    {
        if (isDead) return;

        isDead = true;
        animator.Play("Death", 0, 0f);
        enabled = false;

        StartCoroutine(ShowGameOver());
    }

    private IEnumerator ShowGameOver()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("GAME OVER");
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