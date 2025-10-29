// TutorialManager.cs
using UnityEngine;

public static class TutorialManager
{
    private const string TUTORIAL_COMPLETED_KEY = "TutorialCompleted";

    // Проверяет, первый ли это запуск
    public static bool IsFirstLaunch()
    {
        return PlayerPrefs.GetInt(TUTORIAL_COMPLETED_KEY, 0) == 0;
    }

    // Отмечает, что обучение пройдено
    public static void CompleteTutorial()
    {
        PlayerPrefs.SetInt(TUTORIAL_COMPLETED_KEY, 1);
        PlayerPrefs.Save();
    }

    // (Опционально) Сброс для теста
    public static void ResetTutorial()
    {
        PlayerPrefs.DeleteKey(TUTORIAL_COMPLETED_KEY);
        PlayerPrefs.Save();
    }
}