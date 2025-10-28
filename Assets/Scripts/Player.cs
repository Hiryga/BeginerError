using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }

    [SerializeField] private float moveSpeed = 5f;
    Vector2 inputVector;

    private Rigidbody2D rb;
    private PolygonCollider2D _polygonCollider2D;

    private float minMovingSpeed = 0.1f;
    private bool isRunning = false;

    private bool _isAlive;

    private void Awake() {
        Instance = this;    
        rb = GetComponent<Rigidbody2D>();
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        _isAlive = true;
      
    }

    private void Update() {
        inputVector = GameInput.Instance.GetMovementVector();
    }

    private void FixedUpdate() {
        HandleMovement();
    }

    public bool IsAlive() => _isAlive;

    private void HandleMovement() {
        rb.MovePosition(rb.position + inputVector * (moveSpeed * Time.fixedDeltaTime));
        if (Mathf.Abs(inputVector.x) > minMovingSpeed || Mathf.Abs(inputVector.y) > minMovingSpeed) {
            isRunning = true;
        } else {
            isRunning = false;
        }
    }
    public void OnTrigerEnter2d()
    {
        Debug.Log("Attack");
    }
    public void PolygonColliderTurnOff()
    {
        _polygonCollider2D.enabled = false;
    }

    public void PolygonColliderTurnOn()
    {
        _polygonCollider2D.enabled = true;
    }

    public bool IsRunning() {
        return isRunning;
    }

    public Vector3 GetPlayerScreenPosition() {
        Vector3 playerScrreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return playerScrreenPosition;
    }

  
}
