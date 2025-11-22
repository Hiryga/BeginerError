using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color startColor = new Color(1f, 0f, 0f, 0.2f);
    [SerializeField] private Color endColor = new Color(1f, 0f, 0f, 0.9f);

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
            Debug.Log("[AttackIndicator] Инициализирован. Radius: " + radius + ", Duration: " + duration);
        }
        else
        {
            Debug.LogError("[AttackIndicator] SpriteRenderer не найден!");
        }

        // Начнём с нулевого масштаба
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (!initialized) return;

        if (currentTime < fillTime)
        {
            currentTime += Time.deltaTime;
            float progress = Mathf.Clamp01(currentTime / fillTime);

            // Плавное увеличение размера от центра
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, progress);

            // Плавное изменение цвета и прозрачности
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(startColor, endColor, progress);
            }
        }
    }
}
