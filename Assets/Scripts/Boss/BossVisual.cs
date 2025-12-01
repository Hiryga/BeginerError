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

    // Эти методы вызываются Animation Event'ами
    public void Animation_AttackStart()
    {
        // если хочешь, можно включать доп. коллайдеры/эффекты
    }

    public void Animation_AttackEnd()
    {
        // если атака была завязана на анимацию, тут можно закончить её
    }

    public void Animation_DisableHitCollider()
    {
        _bossEntity?.PolygonColliderTurnOff();
    }

    public void Animation_EnableHitCollider()
    {
        _bossEntity?.PolygonColliderTurnOn();
    }
}
