using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossVisual : MonoBehaviour
{
    [SerializeField] private BossAI _bossAI;
    [SerializeField] private BossEntity _bossEntity;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private const string IS_RUNNING = "IsRunning";
    private const string CHASING_SPEED_MULTIPLIER = "ChasingSpeedMultiplier";
    private const string ATTACK = "Attack";
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
            _bossAI.OnBossAttack += BossAI_OnBossAttack;

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

        // Только параметры для аниматора, без смены направления
        _animator.SetBool(IS_RUNNING, _bossAI.IsRunning);
        _animator.SetFloat(CHASING_SPEED_MULTIPLIER, _bossAI.GetRoamingAnimationSpeed());
    }

    private void BossAI_OnBossAttack(object sender, System.EventArgs e)
    {
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

    // Вызываются из Animation Events (если нужно)
    public void Animation_EnableHitCollider()
    {
        _bossEntity?.PolygonColliderTurnOn();
    }

    public void Animation_DisableHitCollider()
    {
        _bossEntity?.PolygonColliderTurnOff();
    }
}
