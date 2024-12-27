using UnityEngine;

public class JubilantSequenceArchitect : MonoBehaviour
{
    private int _presentEpoch;

    private void Start()
    {
        VitalityMatrixCraft();
        
    }
    
    public void TriumphChime()
    {
        PlayerPrefs.SetInt(EntityFormCoordinator.TemporalEngagementPhase, _presentEpoch+1);
        PlayerPrefs.Save();
        
        var chronicleOverseer = FindObjectOfType<PathVectorStrategist>();
        chronicleOverseer.FulfillStageObjective(_presentEpoch);
    }
    
    private void VitalityMatrixCraft()
    {
        _presentEpoch = PlayerPrefs.GetInt(EntityFormCoordinator.TemporalEngagementPhase, 0);
    }
}