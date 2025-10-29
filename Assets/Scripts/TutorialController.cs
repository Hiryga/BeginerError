using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private float fadeDuration = 1f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = tutorialPanel.AddComponent<CanvasGroup>();

        tutorialPanel.SetActive(false);
    }

    private void Start()
    {
        TutorialManager.ResetTutorial();
        if (TutorialManager.IsFirstLaunch())
        {
            StartCoroutine(ShowTutorialSequence());
        }
    }

    private IEnumerator ShowTutorialSequence()
    {
        yield return new WaitForSeconds(1f);

        // ��� 1: ��������
        yield return StartCoroutine(ShowMessage("��������� WASD ��� �������, ����� ���������", displayTime));

        // ��� 2: �����
        yield return StartCoroutine(ShowMessage("��� � �����. ������ �����!", displayTime));

        // ��� 3: ������
        yield return StartCoroutine(ShowMessage("���� 3 ��������, ����� ������ �������!", displayTime));

        // ���������
        TutorialManager.CompleteTutorial();
    }

    private IEnumerator ShowMessage(string message, float time)
    {
        tutorialText.text = message;
        tutorialPanel.SetActive(true);
        yield return StartCoroutine(FadeIn());

        yield return new WaitForSeconds(time);

        yield return StartCoroutine(FadeOut());
        tutorialPanel.SetActive(false);
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0;
        canvasGroup.alpha = 0;
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
        canvasGroup.alpha = 1;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}