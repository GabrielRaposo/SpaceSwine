using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PagerCallScreen : MonoBehaviour
{
    [Header("Text Codes")]
    [SerializeField] string textCode;
    [SerializeField] string noSignalCode;

    [SerializeField] float updateDelay;

    [Header("References")]
    [SerializeField] GameObject planetAndShip;
    [SerializeField] GameObject noSignalWarning;
    [SerializeField] TextMeshProUGUI callDisplay;

    public bool NoSignalMode;

    private void OnEnable() 
    {
        // Signal Test (verify Story Events)
        //planetAndShip.SetActive()

        if (NoSignalMode)
        {
            
            return;
        }

        StartCoroutine( DisplayLoop() );
    }

    IEnumerator DisplayLoop()
    {
        if (!callDisplay)
            yield break;

        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                callDisplay.text = LocalizationManager.GetUiText(textCode, "calling");

                for (int j = 0; j < i; j++) 
                    callDisplay.text += ".";

                yield return new WaitForSecondsRealtime(updateDelay);
            }
        }
    }

    private void OnDisable() 
    {
        StopAllCoroutines();
    }
}
