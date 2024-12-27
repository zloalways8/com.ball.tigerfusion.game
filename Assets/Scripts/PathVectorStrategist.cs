using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PathVectorStrategist : MonoBehaviour
{
    public static PathVectorStrategist AppGlobalInstance;

    [SerializeField] private GameObject[] _levelOptionButtons;
    [SerializeField] private Button _chooseButton;

    private int _totalStagesCount = 18;
    private int _chosenLevelIndex = 1;

    private void Start()
    {
        
        if (AppGlobalInstance == null)
        {
            AppGlobalInstance = this;
            DontDestroyOnLoad(gameObject);
        }

        ConfigurePlayfields();
        RefreshZoneSelectors();
        _chooseButton.interactable = false;
    }
    
    public void SelectEpoch(int levelIndex)
    {
        _chosenLevelIndex = levelIndex;
        _chooseButton.interactable = true;
    }

    public void ComIn()
    {
        if (_chosenLevelIndex == -1) return;
        
        PlayerPrefs.SetInt(EntityFormCoordinator.TemporalEngagementPhase, _chosenLevelIndex);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene("FericalScene");
    }
    public void CommenceZone()
    {
        if (_chosenLevelIndex == -1) return;
        
        PlayerPrefs.SetInt(EntityFormCoordinator.TemporalEngagementPhase, _chosenLevelIndex);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene(EntityFormCoordinator.QuantumPixelOdyssey);
    }

    public void FulfillStageObjective(int levelIndex)
    {
        PlayerPrefs.SetInt("StageCleared" + levelIndex, 1);
        PlayerPrefs.Save();
        
        if (levelIndex < _totalStagesCount - 1)
        {
            PlayerPrefs.SetInt(EntityFormCoordinator.AscensionTrajectoryMap + levelIndex, 1);
            PlayerPrefs.Save();
        }
        
        RefreshZoneSelectors();
    }

    private void ConfigurePlayfields()
    {
        for (var loopStepCounter = 0; loopStepCounter < _totalStagesCount; loopStepCounter++)
        {
            if (PlayerPrefs.GetInt(EntityFormCoordinator.AscensionTrajectoryMap + loopStepCounter, -1) != -1) continue;
            PlayerPrefs.SetInt(EntityFormCoordinator.AscensionTrajectoryMap + loopStepCounter,
                loopStepCounter == 0 ? 1 : 0);

            PlayerPrefs.SetInt("StageCleared" + loopStepCounter, 0);
        }
        PlayerPrefs.Save();
    }

    private void RefreshZoneSelectors()
    {
        for (var iterationCounter = 0; iterationCounter < _totalStagesCount; iterationCounter++)
        {
            if (_levelOptionButtons[iterationCounter] == null) continue;
            var isLevelUnlocked = PlayerPrefs.GetInt(EntityFormCoordinator.AscensionTrajectoryMap + iterationCounter, 0) == 1;
            var levelButton = _levelOptionButtons[iterationCounter].GetComponent<Button>();
            levelButton.interactable = isLevelUnlocked;
        }
    }
}
