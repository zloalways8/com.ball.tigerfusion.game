using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class HarmonicPulseDirector : MonoBehaviour
{
    [SerializeField] private AudioMixer resonanceCore;
    [SerializeField] private Image waveToggleImage;
    [SerializeField] private Image melodyToggleImage;
    [SerializeField] private Sprite waveOnSprite;
    [SerializeField] private Sprite waveOffSprite;
    [SerializeField] private Sprite melodyOnSprite;
    [SerializeField] private Sprite melodyOffSprite;

    private bool isEchoPulseActive;
    private bool isMelodicFlowActive;

    private const string EchoToggleKey = "sonicEchoToggled";
    private const string MelodicFlowKey = "_melodicFlowActive";

    private void Start()
    {
        LoadPreferences();
        UpdateEchoPulseState();
        UpdateMelodicFlowState();
        SavePreferences();
    }

    public void ToggleEchoPulse()
    {
        isEchoPulseActive = !isEchoPulseActive;
        UpdateEchoPulseState();
        SavePreferences();
    }

    public void ToggleMelodicFlow()
    {
        isMelodicFlowActive = !isMelodicFlowActive;
        UpdateMelodicFlowState();
        SavePreferences();
    }

    private void LoadPreferences()
    {
        isEchoPulseActive = PlayerPrefs.GetInt(EchoToggleKey, 1) == 1;
        isMelodicFlowActive = PlayerPrefs.GetInt(MelodicFlowKey, 1) == 1;
    }

    private void UpdateEchoPulseState()
    {
        resonanceCore.SetFloat(EntityFormCoordinator.EchoResonancePath, isEchoPulseActive ? 0f : -80f);
        waveToggleImage.sprite = isEchoPulseActive ? waveOnSprite : waveOffSprite;
    }

    private void UpdateMelodicFlowState()
    {
        resonanceCore.SetFloat(EntityFormCoordinator.SonicBeaconNode, isMelodicFlowActive ? 0f : -80f);
        melodyToggleImage.sprite = isMelodicFlowActive ? melodyOnSprite : melodyOffSprite;
    }

    private void SavePreferences()
    {
        PlayerPrefs.SetInt(EchoToggleKey, isEchoPulseActive ? 1 : 0);
        PlayerPrefs.SetInt(MelodicFlowKey, isMelodicFlowActive ? 1 : 0);
        PlayerPrefs.Save();
    }
}
