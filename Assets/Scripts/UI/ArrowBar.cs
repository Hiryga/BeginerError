using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ArrowBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform arrowContainer;
    [SerializeField] private Image arrowIconPrefab;
    [SerializeField] private TMP_Text arrowCountText;
    [SerializeField] private Bow bow;

    [Header("Settings")]
    [SerializeField] private float cellSize = 50f;       // Размер каждой стрелы
    [SerializeField] private float spaceBetween = 0f;    // Расстояние между стрелами

    private List<Image> arrowImages = new List<Image>();
    private int lastArrowCount = -1;
    private bool isInitialized = false;

    private void Awake()
    {
        if (bow == null) bow = FindObjectOfType<Bow>();
        if (arrowContainer == null) arrowContainer = transform.Find("ArrowContainer");
        if (arrowIconPrefab == null)
        {
            Transform t = transform.Find("ArrowIcon");
            if (t != null) arrowIconPrefab = t.GetComponent<Image>();
        }
        if (arrowCountText == null)
        {
            Transform t = transform.Find("ArrowCountText");
            if (t != null) arrowCountText = t.GetComponent<TMP_Text>();
        }
    }

    private void Start()
    {
        if (bow == null || arrowContainer == null || arrowIconPrefab == null)
        {
            Debug.LogError("[ArrowBarUI] ❌ Ошибка инициализации!");
            enabled = false;
            return;
        }

        arrowIconPrefab.gameObject.SetActive(false);
        SpawnArrowUI();
        isInitialized = true;
        UpdateArrowUI();
    }

    private void Update()
    {
        if (!isInitialized || bow == null) return;

        int currentArrows = bow.GetArrowCount();
        if (currentArrows != lastArrowCount)
        {
            UpdateArrowUI();
            lastArrowCount = currentArrows;
        }
    }

    private void SpawnArrowUI()
    {
        // Чистим контейнер
        foreach (Transform child in arrowContainer)
        {
            if (child.gameObject != arrowIconPrefab.gameObject)
            {
                Destroy(child.gameObject);
            }
        }
        arrowImages.Clear();

        int maxArrows = bow.maxArrows;

        for (int i = 0; i < maxArrows; i++)
        {
            Image arrowImage = Instantiate(arrowIconPrefab, arrowContainer, false);
            arrowImage.name = $"Arrow_{i}";
            arrowImage.gameObject.SetActive(true);

            RectTransform rectTransform = arrowImage.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(cellSize, cellSize);

            LayoutElement layoutElement = arrowImage.GetComponent<LayoutElement>();
            if (layoutElement == null)
                layoutElement = arrowImage.gameObject.AddComponent<LayoutElement>();

            layoutElement.preferredWidth = cellSize;
            layoutElement.preferredHeight = cellSize;

            arrowImages.Add(arrowImage);
        }

        // ⭐ УДАЛЯЕМ HorizontalLayoutGroup если есть
        HorizontalLayoutGroup oldHLG = arrowContainer.GetComponent<HorizontalLayoutGroup>();
        if (oldHLG != null)
            DestroyImmediate(oldHLG);

        // ⭐ ДОБАВЛЯЕМ/НАСТРАИВАЕМ GridLayoutGroup
        GridLayoutGroup gridLayout = arrowContainer.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
            gridLayout = arrowContainer.gameObject.AddComponent<GridLayoutGroup>();

        gridLayout.cellSize = new Vector2(cellSize, cellSize);
        gridLayout.spacing = new Vector2(spaceBetween, spaceBetween);
        gridLayout.constraintCount = 5;  // 5 СТРЕЛ В РЯДУ (10 стрел = 2 ряда)
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;


        Debug.Log($"[ArrowBarUI] ✅ Сетка 5x2 с размером ячейки {cellSize}x{cellSize}!");
    }

    private void UpdateArrowUI()
    {
        if (arrowImages == null || arrowImages.Count == 0) return;

        int currentArrows = bow.GetArrowCount();

        for (int i = 0; i < arrowImages.Count; i++)
        {
            if (i < currentArrows)
            {
                arrowImages[i].color = Color.white;      // ⬜ Полная
            }
            else
            {
                arrowImages[i].color = new Color(0.4f, 0.4f, 0.4f, 0.6f);  // ⬛ Пустая
            }
        }

        if (arrowCountText != null)
            arrowCountText.text = $"{currentArrows}/{bow.maxArrows}";
    }
}
