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
        if (masterSlider && masterParameter != null)
            masterSlider.InitiateValue( ValueToPercent (masterParameter.GetGlobalValue()) );
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
        SetSliderValue(percent, masterParameter);
    }

    private void SetSliderValue (float percent, AK.Wwise.RTPC parameter)
    {
        if (parameter == null)
            return;

        parameter.SetGlobalValue( PercentToValue(percent) );
    }
}
