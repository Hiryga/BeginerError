using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private Toggle tutorialToggle;  // Можно оставить или убрать, здесь не обязательно
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private float fadeDuration = 1f;

    private CanvasGroup canvasGroup;
    private Coroutine tutorialCoroutine;
    private bool _shouldShowTutorial;

    private void Awake()
    {
        canvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = tutorialPanel.AddComponent<CanvasGroup>();

        tutorialPanel.SetActive(false);

        _shouldShowTutorial = PlayerPrefs.GetInt("ShowTutorial", 1) == 1;

        if (tutorialToggle != null)
        {
            tutorialToggle.isOn = _shouldShowTutorial;
            tutorialToggle.onValueChanged.AddListener(OnToggleChanged);
        }
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

    public void ShowTutorialIfEnabled()
    {
        if (_shouldShowTutorial)
        {
            if (tutorialCoroutine != null)
                StopCoroutine(tutorialCoroutine);

            tutorialCoroutine = StartCoroutine(ShowTutorialSequence());
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

    private IEnumerator ShowTutorialSequence()
    {
        yield return ShowMessage("Используй WASD или стрелки, чтобы двигаться", displayTime);
        yield return ShowMessage("ЛКМ — атака. Целься мышью! Используй '1' и '2' для переключения режима атаки", displayTime);
        yield return ShowMessage("Убей всех монстров, чтобы пройти уровень!", displayTime);
    }

    private IEnumerator ShowMessage(string message, float time)
    {
        tutorialText.text = message;
        tutorialPanel.SetActive(true);
        canvasGroup.alpha = 0;
        yield return FadeIn();
        yield return new WaitForSeconds(time);
        yield return FadeOut();
        tutorialPanel.SetActive(false);
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}
