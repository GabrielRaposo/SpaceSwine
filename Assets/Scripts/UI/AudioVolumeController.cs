using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class AudioVolumeController : MonoBehaviour
{
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider SFXSlider;

    [Space(10)]

    [SerializeField] AK.Wwise.RTPC masterParameter;
    [SerializeField] AK.Wwise.RTPC musicParameter;
    [SerializeField] AK.Wwise.RTPC sfxParameter;

    [Space(10)]

    [Header("Test")]
    [SerializeField] InputAction muteInput;

    [SerializeField] AK.Wwise.Event musicTestEvent;
    [SerializeField] InputAction testInput1;

    [SerializeField] AK.Wwise.Event sfxTestEvent;
    [SerializeField] InputAction testInput2;


    void OnEnable()
    {
        muteInput.performed += (ctx) => 
        {
            if (masterParameter == null)
                return;

            masterParameter.SetGlobalValue( masterParameter.GetGlobalValue() > 0 ? 0 : 100 );
        };
        muteInput.Enable();

        testInput1.performed += (ctx) => 
        {
            musicTestEvent?.Post(gameObject);
        };
        testInput1.Enable();

        testInput2.performed += (ctx) => 
        {
            sfxTestEvent?.Post(gameObject);
        };
        testInput2.Enable();

        if (masterSlider && masterParameter != null)
            masterSlider.value = masterParameter.GetGlobalValue();

        if (musicSlider && musicParameter != null)
            musicSlider.value = musicParameter.GetGlobalValue();

        if (SFXSlider && sfxParameter != null)
            SFXSlider.value = sfxParameter.GetGlobalValue();
    }

    public void SetMasterVolume(float value)
    {
        SetSliderValue (value, masterParameter);
    }

    public void SetMusicVolume(float value)
    {
        SetSliderValue (value, musicParameter);
    }

    public void SetSFXVolume(float value)
    {
        SetSliderValue (value, sfxParameter);
    }

    public void PlayTestSFXSound()
    {
        //AkSoundEngine.PostEvent(WWiseEvent.Collect, gameObject);
    }

    private void SetSliderValue (float value, AK.Wwise.RTPC parameter)
    {
        if (parameter == null)
            return;

        parameter.SetGlobalValue(value);

        //if (parameter == masterParameter)
        //    PlayerPrefsManager.SetFloat( PlayerPrefsManager.Master, value );
        //else if (parameter == musicParameter)
        //    PlayerPrefsManager.SetFloat( PlayerPrefsManager.Music, value );
        //else if (parameter == sfxParameter)
        //    PlayerPrefsManager.SetFloat( PlayerPrefsManager.SFX, value );
    }

    private void OnDisable() 
    {
        muteInput.Disable();
        testInput1.Disable();
        testInput2.Disable();
    }

}

