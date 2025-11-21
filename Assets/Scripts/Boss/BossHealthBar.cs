using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private BossEntity bossEntity;
    [SerializeField] private Image healthFill;

    void Update()
    {
        if (bossEntity != null && healthFill != null)
        {
            float percent = (float)bossEntity.GetCurrentHealth() / bossEntity.GetMaxHealth();
            healthFill.fillAmount = percent; // Image типа Filled
        }
    }
}
