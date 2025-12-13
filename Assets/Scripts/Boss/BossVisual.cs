using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossVisual : MonoBehaviour
{
    [SerializeField] private BossAI _bossAI;
    [SerializeField] private BossEntity _bossEntity;
    [SerializeField] private float baseAttackDuration = 1f; // Базовая длительность анимации атаки

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private const string IS_RUNNING = "IsRunning";
    private const string CHASING_SPEED_MULTIPLIER = "ChasingSpeedMultiplier";
    private const string ATTACK = "Attack";
    private const string ATTACK_SPEED = "AttackSpeed"; // Новый параметр
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
            UpdateAttackSpeed(); // Устанавливаем начальную скорость
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
        if (_bossAI == null) return;

        _animator.SetBool(IS_RUNNING, _bossAI.IsRunning);
        _animator.SetFloat(CHASING_SPEED_MULTIPLIER, _bossAI.GetRoamingAnimationSpeed());
    }

    private void UpdateAttackSpeed()
    {
        if (_bossAI == null) return;

        float attackChargeTime = _bossAI.GetAttackChargeTime(); // Нужно добавить этот метод в BossAI

        // Вычисляем множитель скорости: базовая длительность / время заряда
        float speedMultiplier = baseAttackDuration / attackChargeTime;

        _animator.SetFloat(ATTACK_SPEED, speedMultiplier);
    }

    private void BossAI_OnBossAttack(object sender, System.EventArgs e)
    {
        UpdateAttackSpeed(); // Обновляем скорость перед атакой
        _animator.SetTrigger(ATTACK);
    }

    private void BossEntity_OnTakeHit(object sender, System.EventArgs e)
    {
        _animator.SetTrigger(TAKEHIT);
    }

    private void BossEntity_OnDeath(object sender, System.EventArgs e)
    {
        _animator.SetBool(IS_DIE, true);
    }

    public void Animation_EnableHitCollider()
    {
        _bossEntity?.PolygonColliderTurnOn();
    }

    public void Animation_DisableHitCollider()
    {
        _bossEntity?.PolygonColliderTurnOff();
    }
}
