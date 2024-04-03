using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagerVolumeManager : MonoBehaviour
{
    [SerializeField] AK.Wwise.RTPC masterParameter;
    [SerializeField] AK.Wwise.RTPC musicParameter;
    [SerializeField] AK.Wwise.RTPC sfxParameter;

    [SerializeField] PagerInteractableSlider masterSlider;
    [SerializeField] PagerInteractableSlider musicSlider;
    [SerializeField] PagerInteractableSlider sfxSlider;

    void OnEnable()
    {
        UpdateSliders();
    }

    private void Start()
    {
        UpdateSliders();
    }

    private void UpdateSliders()
    {
        if (masterSlider && masterParameter != null)
            masterSlider.InitiateValue( ValueToPercent (masterParameter.GetGlobalValue()) );

        if (musicSlider && musicParameter != null)
            musicSlider.InitiateValue( ValueToPercent (musicParameter.GetGlobalValue()) );

        if (sfxSlider && sfxParameter != null)
            sfxSlider.InitiateValue( ValueToPercent (sfxParameter.GetGlobalValue()) );
    }

    private float ValueToPercent (float value)
    {
        return value / 100f;
    }

    private float PercentToValue (float value)
    {
        return value * 100f;
    }

    public void SetMasterVolume(float percent)
    {
        SetSliderValue (percent, masterParameter, SoundtrackManager.MASTER_KEY);
    }

    public void SetMusicVolume(float percent)
    {
        SetSliderValue (percent, musicParameter, SoundtrackManager.MUSIC_KEY);
    }

    public void SetSFXVolume(float percent)
    {
        SetSliderValue (percent, sfxParameter, SoundtrackManager.SFX_KEY);
    }

    private void SetSliderValue (float percent, AK.Wwise.RTPC parameter, string key)
    {
        if (parameter == null)
            return;

        parameter.SetGlobalValue( PercentToValue(percent) );
        PlayerPrefs.SetFloat(key, PercentToValue(percent) );
    }
}
