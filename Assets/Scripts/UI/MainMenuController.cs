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
        // Установка состояния галочки из сохранений
        if (tutorialToggle != null)
        {
            tutorialToggle.isOn = PlayerPrefs.GetInt("ShowTutorial", 1) == 1;
            tutorialToggle.onValueChanged.AddListener(OnTutorialToggleChanged);
        }

        if (startButton != null)
            startButton.onClick.AddListener(OnStartGameClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        Time.timeScale = 1f; // На всякий случай, чтобы время было нормальным
    }

    private void OnTutorialToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt("ShowTutorial", isOn ? 1 : 0);
    }

    private void OnStartGameClicked()
    {
        PauseMenu.SetPlayerDead(false); // Сброс флага смерти
        Time.timeScale = 1f; // Сброс времени, чтобы все работало корректно

        // (Иначе атака может заблокироваться!)
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
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
