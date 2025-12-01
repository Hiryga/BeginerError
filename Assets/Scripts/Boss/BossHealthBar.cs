using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFillImage;
    private BossEntity bossEntity;

    private void Awake()
    {
        bossEntity = GetComponentInParent<BossEntity>();
    }

    private void Start()
    {
        if (bossEntity == null)
        {
            Debug.LogError("BossEntity не найден для BossHealthBar!");
            return;
        }

        bossEntity.OnHealthChanged += UpdateBar;
        UpdateBar(null, null);
    }

    private void UpdateBar(object sender, System.EventArgs e)
    {
        if (healthFillImage == null || bossEntity == null) return;
        healthFillImage.fillAmount = bossEntity.GetHealthPercent();
    }

    private void OnDestroy()
    {
        if (bossEntity != null)
            bossEntity.OnHealthChanged -= UpdateBar;
    }
}
