using UnityEngine;
using System.Collections;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Player _player;                         // Ссылка на Player
    [SerializeField] private WeaponSwitcher weaponSwitcher;          // Активное оружие
    [SerializeField] private Sword _sword;                           // Меч
    [SerializeField] private float attackCooldown = 0.5f;

    private Bow activeBow;
    private GameObject _activeWeaponGO; // Сохраняем текущее оружие

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float _nextAttackTime = 0f;
    private bool isAttacking = false;
    private bool isDead = false;
    private const string IS_RUNNING = "IsRunning";
    private const string IS_DEAD = "IsDead";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (weaponSwitcher == null)
        {
            weaponSwitcher = GetComponentInChildren<WeaponSwitcher>();
        }
    }

    private void Start()
    {
        // Сброс состояния игрока при старте сцены
        isDead = false;
        isAttacking = false;
        _nextAttackTime = 0f;

        Player.Instance.GetComponent<PlayerHealth>().OnDeath += HandleDeath;

        // Подписка на события GameInput
        GameInput.Instance.OnAttack += OnAttackInput;      // атака мечом
        GameInput.Instance.OnBowAttack += OnBowAttackInput; // атака из лука
    }


    // МЕЧ
    private void OnAttackInput()
    {
        if (isDead || isAttacking || Time.time < _nextAttackTime || PauseMenu.IsPaused) return;

        UpdateActiveWeapon();

        if (_sword != null && _activeWeaponGO == _sword.gameObject)
        {
            animator.SetTrigger("Attack");
            isAttacking = true;
            _nextAttackTime = Time.time + attackCooldown;
        }
    }

    // ЛУК
    private void OnBowAttackInput()
    {
        if (isDead || isAttacking || Time.time < _nextAttackTime || PauseMenu.IsPaused) return;

        UpdateActiveWeapon();

        // Проверяем наличие стрел перед анимацией
        if (activeBow != null && activeBow.GetArrowCount() > 0)
        {
            animator.SetTrigger("BowAttack");
            isAttacking = true;
            _nextAttackTime = Time.time + attackCooldown;
        }
        // Если стрел нет — ничего не проигрываем!
    }


    private void Update()
    {
        if (isDead) return;

        // Останавливаем движение когда идёт анимация атаки
        if (isAttacking)
        {
            animator.SetBool(IS_RUNNING, false);
            _player.SetCanMove(false); // в Player.cs нужен canMove для блокировки движения
            return;
        }
        else
        {
            _player.SetCanMove(true); // разрешаем движение обратно
        }

        animator.SetBool(IS_RUNNING, Player.Instance.IsRunning());
        AdjustPlayerFacingDirection();
    }

    // Вызови этот метод через Animation Event на последнем кадре BowShot и Attack!
    public void EndAttackAnimation()
    {
        isAttacking = false;
    }

    // BowShot: вызывается через Animation Event (выпуск стрелы)
    public void TriggerBowShootEvent()
    {
        UpdateActiveWeapon();
        if (activeBow != null)
        {
            activeBow.ShootArrowEvent();
        }
    }

    // Меч: Animation Event на включение/выключение коллайдера
    public void TriggerAttackAnimationTurnOn()
    {
        UpdateActiveWeapon();
        if (isDead || _activeWeaponGO != _sword.gameObject) return;
        _sword.AttackColliderTurnOn();
    }

    public void TriggerAttackAnimationTurnOff()
    {
        UpdateActiveWeapon();
        if (isDead || _activeWeaponGO != _sword.gameObject) return;
        _sword.AttackColliderTurnOff();
    }

    private void UpdateActiveWeapon()
    {
        _activeWeaponGO = weaponSwitcher != null ? weaponSwitcher.GetActiveWeapon() : null;
        activeBow = null;
        if (_activeWeaponGO != null) activeBow = _activeWeaponGO.GetComponent<Bow>();

        if (activeBow != null)
            activeBow.IsLookingLeft = spriteRenderer.flipX;
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

    private void AdjustPlayerFacingDirection()
    {
        if (isDead) return;

        // Блокируем поворот, если игра на паузе (меню открыто)
        if (PauseMenu.IsPaused) return;

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
