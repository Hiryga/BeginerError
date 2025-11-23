using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class ArrowBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;           // Картинка внутреннего заполнения (Image fill, например, тип Filled)
    [SerializeField] private TMP_Text arrowText;        // Отображение числа стрел (TextMeshPro)
    [SerializeField] private Bow bow;                   // Bow-компонент игрока

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
        if (bow == null) bow = FindObjectOfType<Bow>();
        if (fillImage == null) fillImage = transform.Find("Fill")?.GetComponent<Image>();
        if (arrowText == null) arrowText = transform.Find("ArrowText")?.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        UpdateArrowUI();
    }

    private void Update()
    {
        // Плавная анимация и перекраска
        int cur = bow.GetArrowCount();
        int max = bow.maxArrows;

        float ratio = cur / (float)max;
        targetFill = ratio;

        currentFill = Mathf.SmoothDamp(currentFill, targetFill, ref velocity, smoothTime);
        if (fillImage != null)
        {
            fillImage.fillAmount = currentFill;

            // По желанию: добавь диапазон для halfColor, если хочешь промежуточный цвет
            if (ratio <= 0.33f)
                fillImage.color = lowColor;
            else if (ratio <= 0.66f)
                fillImage.color = halfColor;
            else
                fillImage.color = fullColor;
        }

        UpdateArrowUI();
    }

    private void UpdateArrowUI()
    {
        int cur = bow.GetArrowCount();
        int max = bow.maxArrows;

        if (arrowText != null)
            arrowText.text = $"{cur}/{max}";
    }

    // Можешь добавить дополнительные методы для реагирования на пополнение стрел
}
