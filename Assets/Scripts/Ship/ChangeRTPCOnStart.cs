using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRTPCOnStart : MonoBehaviour
{
    [SerializeField] AK.Wwise.RTPC fadeOutRTPC;

    void Start()
    {
        if (fadeOutRTPC == null)
            return;

        fadeOutRTPC.SetGlobalValue(0);
    }
}
