using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Tooltip("Имя следующей сцены для загрузки")]
    public string level = "Level2";

    private EnemyEntity[] skeletons;
    private int aliveCount;

    void Start()
    {
        skeletons = FindObjectsOfType<EnemyEntity>();
        aliveCount = skeletons.Length;

        foreach (var skeleton in skeletons)
        {
            if (skeleton != null)
                skeleton.OnDeath += OnSkeletonDeath;
        }
    }

    private void OnSkeletonDeath(object sender, System.EventArgs e)
    {
        aliveCount--;
        if (aliveCount == 0)
        {
            SceneManager.LoadScene(level);
        }
    }

    private void OnDestroy()
    {
        // Важная очистка event-подписок
        foreach (var skeleton in skeletons)
        {
            if (skeleton != null)
                skeleton.OnDeath -= OnSkeletonDeath;
        }
    }
}
