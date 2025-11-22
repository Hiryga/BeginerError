using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFillImage;
    private BossEntity bossEntity;

    private void Awake()
    {
        // Находим BossEntity через родителей
        bossEntity = GetComponentInParent<BossEntity>();
    }

    private void Start()
    {
        if (bossEntity == null)
        {
            Debug.LogError("BossEntity не найден!");
            return;
        }

        // Подписываемся на изменение HP
        bossEntity.OnHealthChanged += UpdateBar;
        UpdateBar(null, null); // Обновляем сразу
    }

    private void UpdateBar(object sender, System.EventArgs e)
    {
        if (healthFillImage == null) return;
        healthFillImage.fillAmount = bossEntity.GetHealthPercent();
    }

    private void OnDestroy()
    {
        if (bossEntity != null)
            bossEntity.OnHealthChanged -= UpdateBar;
    }
}
