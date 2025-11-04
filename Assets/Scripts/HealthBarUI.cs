using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Settings")]
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private Color fullColor = Color.green;
    [SerializeField] private Color halfColor = new Color(1f, 0.8f, 0f);
    [SerializeField] private Color lowColor = Color.red;

    private float targetFill = 1f;
    private float currentFill = 1f;
    private float velocity = 0f;

    private void Awake()
    {
        if (playerHealth == null) playerHealth = FindObjectOfType<PlayerHealth>();
        if (fillImage == null) fillImage = transform.Find("Fill")?.GetComponent<Image>();
        if (hpText == null) hpText = transform.Find("HPText")?.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        UpdateHealth(this, EventArgs.Empty);
    }

    private void OnEnable()
    {
        playerHealth.OnTakeHit += UpdateHealth;
        playerHealth.OnDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        playerHealth.OnTakeHit -= UpdateHealth;
        playerHealth.OnDeath -= OnPlayerDeath;
    }

    private void Update()
    {
        currentFill = Mathf.SmoothDamp(currentFill, targetFill, ref velocity, smoothTime);
        fillImage.fillAmount = currentFill;

        float ratio = playerHealth.GetCurrentHealth() / (float)playerHealth.GetMaxHealth();
        fillImage.color = Color.Lerp(lowColor, fullColor, ratio);
    }

    private void UpdateHealth(object sender, EventArgs e)
    {
        int cur = playerHealth.GetCurrentHealth();
        int max = playerHealth.GetMaxHealth();
        targetFill = cur / (float)max;

        if (hpText != null)
            hpText.text = $"{cur}/{max}";
    }

    private void OnPlayerDeath(object sender, EventArgs e)
    {
        targetFill = 0f;
        if (hpText != null) hpText.text = "0";
    }
}