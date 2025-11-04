using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartPanelController : MonoBehaviour
{
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (restartPanel != null)
            restartPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    public void ShowDeathPanel()
    {
        if (restartPanel != null)
            restartPanel.SetActive(true);

        // При смерти блокируем доступ к паузе
        PauseMenu.SetPlayerDead(true);
    }

    private void RestartGame()
    {
        // Разблокируем паузу при рестарте
        PauseMenu.SetPlayerDead(false); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
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
