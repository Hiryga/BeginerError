using UnityEngine;

public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 2.5f, 0);

    private Transform bossTransform;

    private void Awake()
    {
        // Идём вверх на 2 уровня: CanvasBossHp -> BossVisual -> FirstBoss
        bossTransform = transform.parent.parent;
    }

    private void LateUpdate()
    {
        if (bossTransform == null) return;
        transform.position = bossTransform.position + offset;
    }
}
