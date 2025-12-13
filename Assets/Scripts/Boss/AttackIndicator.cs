using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color startColor = new Color(1f, 0f, 0f, 0.05f);
    [SerializeField] private Color endColor = new Color(1f, 0f, 0f, 0.7f);

    private float fillTime = 1f;
    private float currentTime = 0f;
    private bool initialized = false;

    public void Initialize(float duration, float radius)
    {
        fillTime = duration;
        Vector3 targetScale = new Vector3(radius * 2, radius * 2, 1f);
        initialized = true;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = startColor;
            // ИСПРАВЛЕНИЕ: сразу устанавливаем полный размер
            transform.localScale = targetScale;
        }
        else
        {
            Debug.LogError("[AttackIndicator] SpriteRenderer не найден!");
        }
    }

    private void Update()
    {
        if (!initialized || spriteRenderer == null) return;

        if (currentTime < fillTime)
        {
            currentTime += Time.deltaTime;
            float progress = Mathf.Clamp01(currentTime / fillTime);

            // только прозрачность меняется
            spriteRenderer.color = Color.Lerp(startColor, endColor, progress);
        }
    }
}
