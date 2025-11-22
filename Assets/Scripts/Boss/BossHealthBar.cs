using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private BossEntity bossEntity;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private bool hideWhenFull = false;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        if (bossEntity != null)
        {
            bossEntity.OnHealthChanged += BossEntity_OnHealthChanged;
            UpdateHealthBar();
        }
    }

    private void OnDestroy()
    {
        if (bossEntity != null)
        {
            bossEntity.OnHealthChanged -= BossEntity_OnHealthChanged;
        }
    }

    private void BossEntity_OnHealthChanged(object sender, System.EventArgs e)
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthFillImage != null && bossEntity != null)
        {
            float healthPercent = bossEntity.GetHealthPercent();
            healthFillImage.fillAmount = healthPercent;

            // Скрыть полоску если HP полное
            if (hideWhenFull && canvas != null)
            {
                canvas.enabled = healthPercent < 1f;
            }
        }
    }
}
