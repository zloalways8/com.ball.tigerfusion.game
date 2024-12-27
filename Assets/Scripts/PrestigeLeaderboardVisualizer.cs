using UnityEngine;
using TMPro;

public class PrestigeLeaderboardVisualizer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _highScoreTexts; // Массив для TextMeshProUGUI элементов для отображения рекордов
    [SerializeField] private TextMeshProUGUI[] _highScoreDates; // Массив для TextMeshProUGUI элементов для отображения рекордов
    private ApexScoreRegistrar apexScoreRegistrar;

    void Start()
    {
        
        apexScoreRegistrar = FindObjectOfType<ApexScoreRegistrar>();

        // Обновляем UI с максимальными рекордами
        UpdateHighScoreDisplay();
    }

    void UpdateHighScoreDisplay()
    {
        int[] highScores = apexScoreRegistrar.GetHighScores();
        string[] highScoreDates = apexScoreRegistrar.GetHighScoreDates();

        // Обновляем текст для каждого рекорда
        for (int i = 0; i < _highScoreTexts.Length; i++)
        {
            _highScoreTexts[i].text = $"{highScores[i]}";
            _highScoreDates[i].text = $"{highScoreDates[i]}";
        }
    }
}