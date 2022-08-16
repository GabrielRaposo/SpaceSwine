using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationData : MonoBehaviour
{
    [SerializeField] string locationCode;
    [SerializeField] string fallbackText;

    float delay = 1.5f;

    void Start()
    {
        LocationDisplay display = LocationDisplay.Instance;
        if (!display)
            return;

        string t = LocalizationManager.GetUiText(locationCode, fallbackText);

        RaposUtil.WaitSeconds(this, delay, () => 
            {
                display.DisplayLocation(t);
            }
        );
    }

}
