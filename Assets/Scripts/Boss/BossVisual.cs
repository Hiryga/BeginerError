using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossVisual : MonoBehaviour
{
    [SerializeField] private BossAI _bossAI;
    [SerializeField] private BossEntity _bossEntity;
    [SerializeField] private float baseAttackDuration = 1f;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private bool _isAttacking = false;
    private bool _isDead = false; // Флаг смерти

    private const string IS_RUNNING = "IsRunning";
    private const string CHASING_SPEED_MULTIPLIER = "ChasingSpeedMultiplier";
    private const string ATTACK = "Attack";
    private const string ATTACK_SPEED = "AttackSpeed";
    private const string TAKEHIT = "TakeHit";
    private const string IS_DIE = "IsDie";

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (_bossAI != null)
        {
            _bossAI.OnBossAttack += BossAI_OnBossAttack;
            UpdateAttackSpeed();
        }

        if (_bossEntity != null)
        {
            _bossEntity.OnTakeHit += BossEntity_OnTakeHit;
            _bossEntity.OnDeath += BossEntity_OnDeath;
        }
    }

    private void OnDestroy()
    {
        if (_bossAI != null)
            _bossAI.OnBossAttack -= BossAI_OnBossAttack;

        if (_bossEntity != null)
        {
            _bossEntity.OnTakeHit -= BossEntity_OnTakeHit;
            _bossEntity.OnDeath -= BossEntity_OnDeath;
        }
    }

    private void Update()
    {
        if (_bossAI == null || _isDead) return;

        _animator.SetBool(IS_RUNNING, _bossAI.IsRunning);
        _animator.SetFloat(CHASING_SPEED_MULTIPLIER, _bossAI.GetRoamingAnimationSpeed());
    }

    private void UpdateAttackSpeed()
    {
        if (_bossAI == null) return;

        float attackChargeTime = _bossAI.GetAttackChargeTime();
        float speedMultiplier = baseAttackDuration / attackChargeTime;
        _animator.SetFloat(ATTACK_SPEED, speedMultiplier);
    }

    private void BossAI_OnBossAttack(object sender, System.EventArgs e)
    {
        if (_isDead) return; // Не атакуем, если мёртв

        UpdateAttackSpeed();
        _animator.SetTrigger(ATTACK);
        _isAttacking = true;
    }

    private void BossEntity_OnTakeHit(object sender, System.EventArgs e)
    {
        // КЛЮЧЕВОЕ ИСПРАВЛЕНИЕ: Блокируем TakeHit во время атаки или смерти
        if (!_isAttacking && !_isDead)
        {
            _animator.SetTrigger(TAKEHIT);
        }
    }
    private void BossEntity_OnDeath(object sender, System.EventArgs e)
    {
        _isDead = true;
        _isAttacking = false;

        // Сбрасываем все триггеры и bool параметры
        _animator.ResetTrigger(TAKEHIT);
        _animator.ResetTrigger(ATTACK);
        _animator.SetBool(IS_RUNNING, false); // Останавливаем ходьбу

        // Устанавливаем состояние смерти
        _animator.SetBool(IS_DIE, true);

        // Принудительно играем анимацию смерти (на случай если переходы не работают)
        _animator.Play("BossDeath", 0, 0f);

        Debug.Log("[BossVisual] Анимация смерти запущена");
    }

    // Вызывается из Animation Event в конце анимации атаки
    public void Animation_AttackEnd()
    {
        _isAttacking = false;
    }

    public void Animation_EnableHitCollider()
    {
        if (_bossEntity != null && !_isDead)
        {
            _bossEntity.PolygonColliderTurnOn();
        }
    }

    public void Animation_DisableHitCollider()
    {
        _bossEntity?.PolygonColliderTurnOff();
    }
}
