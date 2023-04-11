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
}
