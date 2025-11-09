using UnityEngine;

public class Bow : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;           // Префаб стрелы (Arrow)
    [SerializeField] private Transform arrowSpawnPoint;        // Просто точка для спауна (Transform)
    [SerializeField] private float arrowSpeed = 12f;

    // Этот метод вызывается через Animation Event из PlayerVisual
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
