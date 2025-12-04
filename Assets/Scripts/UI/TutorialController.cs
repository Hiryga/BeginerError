using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private Toggle tutorialToggle;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.25f;       // можно 0, если не нужен фейд
    [SerializeField] private InputAction skipAction;           // действие (F) из нового Input System

    private CanvasGroup canvasGroup;
    private Coroutine tutorialCoroutine;
    private bool _shouldShowTutorial;
    private bool _skipRequested;

    // Показан ли уже туториал в ЭТОЙ сцене (за текущий запуск игры)
    private bool _tutorialShownThisSession = false;

    // Тексты шагов
    private readonly string[] _messages =
    {
        "Используй WASD или стрелки, чтобы двигаться",
        "ЛКМ — атака. Целься мышью! Используй '1' и '2' для переключения режима атаки",
        "Убей всех монстров, чтобы пройти уровень!"
    };

    private void Awake()
    {
        canvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = tutorialPanel.AddComponent<CanvasGroup>();

        tutorialPanel.SetActive(false);

        // Просто читаем настройку игрока — показывать обучение или нет
        _shouldShowTutorial = PlayerPrefs.GetInt("ShowTutorial", 1) == 1;

        if (tutorialToggle != null)
        {
            tutorialToggle.isOn = _shouldShowTutorial;
            tutorialToggle.onValueChanged.AddListener(OnToggleChanged);
        }

        // Подписка на действие пропуска (например, клавиша F)
        skipAction.Enable();
        skipAction.performed += OnSkipPerformed;
    }

    private void OnDestroy()
    {
        skipAction.performed -= OnSkipPerformed;
        skipAction.Disable();
    }

    private void Start()
    {
        ShowTutorialIfEnabled();
    }

    private void OnToggleChanged(bool isOn)
    {
        _shouldShowTutorial = isOn;
        PlayerPrefs.SetInt("ShowTutorial", isOn ? 1 : 0);
    }

    private void OnSkipPerformed(InputAction.CallbackContext ctx)
    {
        _skipRequested = true;
    }

    /// <summary>
    /// Вызывается при старте сцены и из PauseMenu.Resume().
    /// </summary>
    public void ShowTutorialIfEnabled()
    {
        // Если в этой сцене уже показали один раз — больше не запускаем
        if (_tutorialShownThisSession)
            return;

        if (_shouldShowTutorial)
        {
            if (tutorialCoroutine != null)
                StopCoroutine(tutorialCoroutine);

            tutorialCoroutine = StartCoroutine(TutorialDialogue());
        }
        else
        {
            tutorialPanel.SetActive(false);
            if (tutorialCoroutine != null)
            {
                StopCoroutine(tutorialCoroutine);
                tutorialCoroutine = null;
            }
        }
    }

    private IEnumerator TutorialDialogue()
    {
        tutorialPanel.SetActive(true);

        for (int i = 0; i < _messages.Length; i++)
        {
            _skipRequested = false;

            tutorialText.text = _messages[i];

            // Плавно показать
            yield return Fade(0f, 1f);

            // Ждём нажатия F
            while (!_skipRequested)
                yield return null;

            // Плавно скрыть перед следующим сообщением
            yield return Fade(1f, 0f);
        }

        tutorialPanel.SetActive(false);
        tutorialCoroutine = null;

        // В этой сцене туториал уже показан – больше не запускать до перезагрузки сцены
        _tutorialShownThisSession = true;
    }

    private IEnumerator Fade(float from, float to)
    {
        if (fadeDuration <= 0f)
        {
            canvasGroup.alpha = to;
            yield break;
        }

        float elapsed = 0f;
        canvasGroup.alpha = from;

        // unscaledDeltaTime, чтобы фейд работал и на паузе
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}
