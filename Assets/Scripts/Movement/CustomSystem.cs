using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CustomEase
{
    None,
    Linear,
    EaseIn,
    EaseOut,
    EaseInOut,
    CubicEaseIn,
    CubicEaseOut
}

public static class CustomSystem
{
    public static float SetEase(this float t, CustomEase ease)
    {
        switch (ease) 
        {
            default:
            case CustomEase.Linear:
                return t;

            case CustomEase.EaseIn:
                return Mathf.Pow(t, 2);

            case CustomEase.EaseOut:
                return 1 - Mathf.Pow(t - 1, 2);

            case CustomEase.EaseInOut:
                return Mathf.Pow(t, 2) * (3 - (2 * t));

            case CustomEase.CubicEaseIn:
                return Mathf.Pow(t, 3);

            case CustomEase.CubicEaseOut:
                return Mathf.Pow(t - 1, 3) + 1;
        }
    }

}
