using UnityEngine;

public class Bow : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float arrowSpeed = 12f;

    private Vector3 originalSpawnLocalPos;
    public bool IsLookingLeft { get; set; } = false;

    private void Awake()
    {
        if (arrowSpawnPoint != null)
            originalSpawnLocalPos = arrowSpawnPoint.localPosition;
    }

    private void Update()
    {
        if (arrowSpawnPoint == null) return;

        // Зеркально меняем позицию точки спавна по оси X в локальных координатах
        Vector3 newLocalPos = originalSpawnLocalPos;
        if (IsLookingLeft)
            newLocalPos.x = -Mathf.Abs(newLocalPos.x);
        else
            newLocalPos.x = Mathf.Abs(newLocalPos.x);

        arrowSpawnPoint.localPosition = newLocalPos;
    }

    public void ShootArrowEvent()
    {
        if (arrowSpawnPoint == null || arrowPrefab == null) return;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        Vector2 shootDirection = (mouseWorldPosition - arrowSpawnPoint.position).normalized;

        GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

        Arrow arrowScript = newArrow.GetComponent<Arrow>();
        if (arrowScript != null)
            arrowScript.Initialize(shootDirection, arrowSpeed);
    }
}
