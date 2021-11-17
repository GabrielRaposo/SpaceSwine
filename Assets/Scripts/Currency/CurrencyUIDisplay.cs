using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(HideDisplayPosition))]
public class CurrencyUIDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI valueDisplay;

    HideDisplayPosition hideDisplayPosition;

    private void OnEnable() 
    {
        hideDisplayPosition = GetComponent<HideDisplayPosition>();

        PlayerWallet.OnValueUpdate += (f) => OnValueChange(f, true);

        int worldId = 1;
        OnValueChange(PlayerWallet.GetValueBy(worldId));
    }

    private void OnValueChange (float value, bool showDisplay = false)
    {
        if (!valueDisplay)
            return;

        if (showDisplay) 
            hideDisplayPosition.Show();
        
        valueDisplay.text = "C: " + value.ToString("0");
    }

    private void OnDisable() 
    {
        PlayerWallet.OnValueUpdate -= (f) => OnValueChange(f, true);
    }
}
