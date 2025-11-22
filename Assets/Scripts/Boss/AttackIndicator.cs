using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color startColor = new Color(1f, 0f, 0f, 0.05f); // Очень блеклый (5%)
    [SerializeField] private Color endColor = new Color(1f, 0f, 0f, 0.7f);    // До 70%

    private float fillTime = 1f;
    private float currentTime = 0f;
    private Vector3 targetScale;
    private bool initialized = false;

    public void Initialize(float duration, float radius)
    {
        fillTime = duration;
        targetScale = new Vector3(radius * 2, radius * 2, 1f);
        initialized = true;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = startColor;
            transform.localScale = targetScale;
            Debug.Log("[AttackIndicator] ✓ Инициализирован. Radius: " + radius + ", Duration: " + duration);
        }
        else
        {
            Debug.LogError("[AttackIndicator] ✗ SpriteRenderer не найден!");
        }
    }

    private void Update()
    {
        if (!initialized) return;

        if (currentTime < fillTime)
        {
            currentTime += Time.deltaTime;
            float progress = Mathf.Clamp01(currentTime / fillTime);

            // Только прозрачность меняется от 5% до 70%
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(startColor, endColor, progress);
            }
        }
    }
}
