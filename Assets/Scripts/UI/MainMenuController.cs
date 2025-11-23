using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Toggle tutorialToggle;

    private void Awake()
    {
        // ”становка состо€ни€ галочки из сохранений
        if (tutorialToggle != null)
        {
            tutorialToggle.isOn = PlayerPrefs.GetInt("ShowTutorial", 1) == 1;
            tutorialToggle.onValueChanged.AddListener(OnTutorialToggleChanged);
        }

        if (startButton != null)
            startButton.onClick.AddListener(OnStartGameClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        Time.timeScale = 1f; // Ќа вс€кий случай, чтобы врем€ было нормальным
    }

    private void OnTutorialToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt("ShowTutorial", isOn ? 1 : 0);
    }

    private void OnStartGameClicked()
    {
        SceneManager.LoadScene("SampleScene");
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
