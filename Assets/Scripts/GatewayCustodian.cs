using UnityEngine;
using UnityEngine.SceneManagement;

public class GatewayCustodian : MonoBehaviour
{
    public void LoadLimboRealm()
    {
        LoadRealm(EntityFormCoordinator.QuantumPixelOdyssey);
    }

    public void LoadVoxelOdysseyRealm()
    {
        LoadRealm(EntityFormCoordinator.TransitionalRealmNexus);
    }

    private void LoadRealm(string realmName)
    {
        SceneManager.LoadScene(realmName);
    }
}