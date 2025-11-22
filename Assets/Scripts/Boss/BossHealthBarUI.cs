using UnityEngine;

public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 2.5f, 0);

    private Transform bossTransform;
    private Camera cam;
    private Canvas canvas;

    private void Awake()
    {
        cam = Camera.main;
        canvas = GetComponent<Canvas>();

        // ВАЖНО: Находим босса через родителя Canvas
        // Canvas находится внутри CanvasBossHp -> BossVisual -> FirstBoss
        // Поэтому идем на 2 уровня вверх
        if (transform.parent != null && transform.parent.parent != null)
        {
            bossTransform = transform.parent.parent; // FirstBoss
            Debug.Log($"[BossHealthBarUI] Boss найден: {bossTransform.name}");
        }
        else
        {
            Debug.LogError("[BossHealthBarUI] Не удалось найти босса! Проверьте иерархию.");
        }

        if (canvas != null)
        {
            canvas.enabled = true;
        }
    }

    private void LateUpdate()
    {
        if (bossTransform != null && cam != null)
        {
            // Полоска следует за боссом с оффсетом
            transform.position = bossTransform.position + offset;
            transform.rotation = cam.transform.rotation;
        }
    }
}
