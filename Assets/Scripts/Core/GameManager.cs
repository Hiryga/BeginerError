using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public string level = "Level2";
    public GameObject levelCompletePanel; // UI-панель с сообщением

    private EnemyEntity[] skeletons;
    private int aliveCount;
    private bool isLevelComplete = false;

    void Start()
    {
        skeletons = FindObjectsOfType<EnemyEntity>();
        aliveCount = skeletons.Length;

        foreach (var skeleton in skeletons)
        {
            if (skeleton != null)
                skeleton.OnDeath += OnSkeletonDeath;
        }

        levelCompletePanel.SetActive(false);
    }

    private void OnSkeletonDeath(object sender, System.EventArgs e)
    {
        aliveCount--;
        if (aliveCount == 0 && !isLevelComplete)
        {
            isLevelComplete = true;
            StartCoroutine(ShowLevelCompleteAndLoadNext());
        }
    }

    private IEnumerator ShowLevelCompleteAndLoadNext()
    {
        levelCompletePanel.SetActive(true);
        yield return new WaitForSeconds(3); // ѕоказываем 3 секунды
        SceneManager.LoadScene(level);
    }

    private void OnDestroy()
    {
        foreach (var skeleton in skeletons)
        {
            if (skeleton != null)
                skeleton.OnDeath -= OnSkeletonDeath;
        }
    }
}
