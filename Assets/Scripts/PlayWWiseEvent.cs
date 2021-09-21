using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayWWiseEvent : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event audioEvent;
    [SerializeField] float delay;

    void Start()
    {
        if (audioEvent == null)
            return;

        StartCoroutine
        (
            RaposUtil.WaitSeconds(delay, () => audioEvent.Post(gameObject))
        );
    }
}
