using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused { get; private set; }
    public static bool IsPlayerDead { get; private set; } = false;

    [Header("UI Elements")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TutorialController tutorialController;


    private bool isPaused = false;

    private void Awake()
    {
        pauseMenuUI.SetActive(false);
        resumeButton.onClick.AddListener(Resume);
        quitButton.onClick.AddListener(QuitGame);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(BackToMainMenu);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPlayerDead) return;

            if (!isPaused)
                Pause();
            else
                Resume();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        IsPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        IsPaused = false;

        // !!! После выхода из меню всегда проверяем и запускаем туториал если галочка активна
        if (tutorialController != null)
            tutorialController.ShowTutorialIfEnabled();
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void BackToMainMenu()
    {
        Time.timeScale = 1f; // Сброс времени, если был поставлен на паузу
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }


    public static void SetPlayerDead(bool dead)
    {
        IsPlayerDead = dead;
        IsPaused = false;    // Снимаем любую паузу при сбросе
    }

}
