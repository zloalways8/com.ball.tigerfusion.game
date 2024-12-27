using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApexScoreRegistrar : MonoBehaviour
{
    private const string ScoreKeyPrefix = "HighScore_";
    private const string DateKeyPrefix = "HighScoreDate_";

    private int[] highScores = new int[4];
    private string[] highScoreDates = new string[4];

    void Start()
    {
        LoadHighScores();
    }

    // Загружаем высокие рекорды из PlayerPrefs
    public void LoadHighScores()
    {
        for (int i = 0; i < 4; i++)
        {
            highScores[i] = PlayerPrefs.GetInt(ScoreKeyPrefix + i, 0);
            highScoreDates[i] = PlayerPrefs.GetString(DateKeyPrefix + i, "No Date");
        }
    }
    public void ClearHighScores()
    {
        // Сбрасываем все рекорды и даты
        for (int i = 0; i < 4; i++)
        {
            highScores[i] = 0;
            highScoreDates[i] = "No Date";

            // Удаляем значения из PlayerPrefs
            PlayerPrefs.DeleteKey(ScoreKeyPrefix + i);
            PlayerPrefs.DeleteKey(DateKeyPrefix + i);
        }

        // Сохраняем изменения в PlayerPrefs
        PlayerPrefs.Save();
        SceneManager.LoadScene(EntityFormCoordinator.TransitionalRealmNexus);
    }
    // Сохраняем новый рекорд с датой
    public void SaveNewHighScore(int newScore)
    {
        DateTime currentDate = DateTime.Now;
        string formattedDate = currentDate.ToString("dd.MM.yy"); // Форматируем дату как "14.12.2024"

        // Проходим по всем рекордам и ищем, куда можно вставить новый рекорд
        for (int i = 0; i < 4; i++)
        {
            if (newScore > highScores[i])
            {
                // Перемещаем все рекорды вниз, начиная с последнего
                for (int j = 3; j > i; j--)
                {
                    highScores[j] = highScores[j - 1];
                    highScoreDates[j] = highScoreDates[j - 1];
                }

                // Вставляем новый рекорд на его место
                highScores[i] = newScore;
                highScoreDates[i] = formattedDate;

                // Сохраняем в PlayerPrefs
                for (int k = 0; k < 4; k++)
                {
                    PlayerPrefs.SetInt(ScoreKeyPrefix + k, highScores[k]);
                    PlayerPrefs.SetString(DateKeyPrefix + k, highScoreDates[k]);
                }

                PlayerPrefs.Save();
                break;
            }
        }
    }



    // Получаем все высокие рекорды
    public int[] GetHighScores()
    {
        return highScores;
    }

    public string[] GetHighScoreDates()
    {
        return highScoreDates;
    }
}