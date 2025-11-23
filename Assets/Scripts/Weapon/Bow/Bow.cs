using UnityEngine;
using UnityEngine.UI; // Если нужна UI для отображения количества стрел

public class Bow : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float arrowSpeed = 12f;
    [SerializeField] public int maxArrows = 10; // Максимальное количество стрел
    [SerializeField] private Text arrowCountText; // (Опционально) для UI

    private Vector3 originalSpawnLocalPos;
    public bool IsLookingLeft { get; set; } = false;
    private int currentArrows;

    private void Awake()
    {
        if (arrowSpawnPoint != null)
            originalSpawnLocalPos = arrowSpawnPoint.localPosition;
        currentArrows = maxArrows;
        UpdateArrowCountUI();
    }

    private void Update()
    {
        if (arrowSpawnPoint == null) return;

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

        // Проверяем, есть ли стрелы
        if (currentArrows <= 0)
        {
            Debug.Log("[Bow] Нет стрел для выстрела!");
            return;
        }

        // Выстрел разрешён — уменьшаем количество стрел
        currentArrows--;
        UpdateArrowCountUI();

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        Vector2 shootDirection = (mouseWorldPosition - arrowSpawnPoint.position).normalized;

        GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

        Arrow arrowScript = newArrow.GetComponent<Arrow>();
        if (arrowScript != null)
            arrowScript.Initialize(shootDirection, arrowSpeed);
    }

    private void UpdateArrowCountUI()
    {
        if (arrowCountText != null)
            arrowCountText.text = $"Arrows: {currentArrows}";
    }

    // Метод для пополнения стрел (если понадобится)
    public void AddArrows(int count)
    {
        currentArrows = Mathf.Clamp(currentArrows + count, 0, maxArrows);
        UpdateArrowCountUI();
    }

    // Можно добавить получение числа стрел, если нужно для других скриптов:
    public int GetArrowCount() => currentArrows;
}
