using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }
    public event EventHandler OnPlayerDeath;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float damageRecoveryTime = 0.5f;
    Vector2 inputVector;

    private Rigidbody2D _rb;
    private KnockBack _knockBack;

    private float minMovingSpeed = 0.1f;
    private bool isRunning = false;

    private int _currentHealth;
    private bool _canTakeDamage;
    private bool _isAlive;


    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
        _knockBack = GetComponent<KnockBack>();
    }

    private void Start()
    {
        _currentHealth = maxHealth;
        _canTakeDamage = true;
        _isAlive = true;

   
    }

   

    public void TakeDamage(Transform damageSource, int damage)
    {
        if (_canTakeDamage && _isAlive)
        {
            _canTakeDamage = false;
            _currentHealth = Mathf.Max(0, _currentHealth -= damage);
            Debug.Log(_currentHealth);
            _knockBack.GetKnockedBack(damageSource);


            StartCoroutine(DamageRecoveryRoutine());
        }

        DetectDeath();
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        _canTakeDamage = true;
    }

    private void DetectDeath()
    {
        if (_currentHealth == 0 && _isAlive)
        {
            _isAlive = false;
            _knockBack.StopKnockBackMovement();
            GameInput.Instance.DisablePlayerInput();
            FindObjectOfType<RestartPanelController>().ShowDeathPanel();
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        }

    }


    private void Update() {
        inputVector = GameInput.Instance.GetMovementVector();
    }

    private void FixedUpdate()
    {
        if (_knockBack.IsGettingKnockedBack)
            return;

        HandleMovement();
    }

    public bool IsAlive() => _isAlive;

    private void HandleMovement() {
        _rb.MovePosition(_rb.position + inputVector * (moveSpeed * Time.fixedDeltaTime));
        if (Mathf.Abs(inputVector.x) > minMovingSpeed || Mathf.Abs(inputVector.y) > minMovingSpeed) {
            isRunning = true;
        } else {
            isRunning = false;
        }
    }


    public bool IsRunning() {
        return isRunning;
    }

    public Vector3 GetPlayerScreenPosition() {
        Vector3 playerScrreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return playerScrreenPosition;
    }

   

}