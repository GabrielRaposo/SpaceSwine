using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSurface : MonoBehaviour
{
    [Header("Custom Surface")]
    [SerializeField] AK.Wwise.Event stepsAKEvent;
    [SerializeField] AK.Wwise.Event landAKEvent;
    [SerializeField] ParticleSystem landEffect;
    //[Space(10)]

    public void PlayStepEffects()
    {
        if (stepsAKEvent != null)
            stepsAKEvent.Post(gameObject);
    }

    public void PlayLandEffects()
    {
        if (landAKEvent != null)
            landAKEvent.Post(gameObject);

        // PLay land effect
    }
}
