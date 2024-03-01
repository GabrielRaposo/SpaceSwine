using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PagerCallScreen : MonoBehaviour
{
    [Header("Text Codes")]
    [SerializeField] string textCode;
    [SerializeField] string noSignalCode;
    [SerializeField] string resetCode;

    [SerializeField] float updateDelay;

    [Header("References")]
    [SerializeField] GameObject planetAndShip;
    [SerializeField] GameObject noSignalWarning;
    [SerializeField] GameObject resettingSave;
    [SerializeField] TextMeshProUGUI callDisplay;

    [HideInInspector] public bool NoSignalMode;
    [HideInInspector] public bool titleScreenMode;

    private void OnEnable() 
    {
        planetAndShip.SetActive(!NoSignalMode && !titleScreenMode);
        noSignalWarning.SetActive(NoSignalMode && !titleScreenMode);
        resettingSave.SetActive(titleScreenMode);

        if (titleScreenMode)
        {
            StartCoroutine ( DisplayResetLoop() );
            return;
        }

        if (NoSignalMode)
        {
            StartCoroutine( DisplayErrorLoop() );
            return;
        }

        StartCoroutine( DisplayCallLoop() );
    }

    IEnumerator DisplayCallLoop()
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

    IEnumerator DisplayErrorLoop()
    {
        if (!callDisplay)
            yield break;

        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                callDisplay.text = LocalizationManager.GetUiText(noSignalCode, "no signal");

                for (int j = 0; j < i; j++) 
                    callDisplay.text += ".";

                yield return new WaitForSecondsRealtime(updateDelay);
            }
        }
    }

    IEnumerator DisplayResetLoop()
    {
        if (!callDisplay)
            yield break;

        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                callDisplay.text = LocalizationManager.GetUiText(resetCode, "resetting save");

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
