using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public string level = "Level2";
    public GameObject levelCompletePanel; // UI-панель с сообщением
    public GameObject gameCompletePanel; // UI-панель с сообщением о завершении игры
    public string bossSceneName = "BossLevel"; // Название сцены с боссом

    private EnemyEntity[] enemies;
    private int aliveCount;
    private bool isLevelComplete = false;

    void Start()
    {
        enemies = FindObjectsOfType<EnemyEntity>();
        aliveCount = enemies.Length;

        foreach (var enemy in enemies)
        {
            if (enemy != null)
                enemy.OnDeath += OnEnemyDeath;
        }

        levelCompletePanel.SetActive(false);
        if (gameCompletePanel != null)
            gameCompletePanel.SetActive(false);
    }

    private void OnEnemyDeath(object sender, System.EventArgs e)
    {
        aliveCount--;
        if (aliveCount == 0 && !isLevelComplete)
        {
            isLevelComplete = true;

            // Проверяем, является ли текущая сцена боссовой
            if (SceneManager.GetActiveScene().name == bossSceneName)
            {
                StartCoroutine(ShowGameComplete());
            }
            else
            {
                StartCoroutine(ShowLevelCompleteAndLoadNext());
            }
        }
    }

    private IEnumerator ShowLevelCompleteAndLoadNext()
    {
        levelCompletePanel.SetActive(true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(level);
    }

    private IEnumerator ShowGameComplete()
    {
        if (gameCompletePanel != null)
        {
            gameCompletePanel.SetActive(true);
            // Здесь можно добавить переход в главное меню или показать статистику
            yield return new WaitForSeconds(5);
            // SceneManager.LoadScene("MainMenu"); // По желанию
        }
    }

    private void OnDestroy()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                enemy.OnDeath -= OnEnemyDeath;
        }
    }
}
