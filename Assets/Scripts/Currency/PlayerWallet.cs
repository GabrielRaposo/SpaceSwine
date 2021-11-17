using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerWallet : MonoBehaviour
{
    public static UnityAction <float> OnValueUpdate; 

    private static float world1Currency;
    //public static float world2Currency;
    //public static float world3Currency;

    private static float digitalCurrency;

    public static void ChangeValue (float value, int worldId)
    {
        switch (worldId)
        {
            default:
            case 1:
                world1Currency += value;
                OnValueUpdate?.Invoke(world1Currency);
                break;

            case 2:
                break;

            case 3:
                break;
        }
    }

    public static float GetValueBy(int worldId)
    {
        switch (worldId)
        {
            default:
            case 1:
                return world1Currency;

            //case 2:
            //    return world2Currency;

            //case 3:
            //    return world3Currency;
        }
    }
}
