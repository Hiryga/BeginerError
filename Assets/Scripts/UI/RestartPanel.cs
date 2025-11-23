using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartPanelController : MonoBehaviour
{
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (restartPanel != null)
            restartPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(BackToMainMenu);
    }

    public void ShowDeathPanel()
    {
        if (restartPanel != null)
            restartPanel.SetActive(true);

        // ѕри смерти блокируем доступ к паузе
        PauseMenu.SetPlayerDead(true);
    }

    private void RestartGame()
    {
        // –азблокируем паузу при рестарте
        PauseMenu.SetPlayerDead(false); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    private void BackToMainMenu()
    {
        Time.timeScale = 1f; // —брос времени, если был поставлен на паузу
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
