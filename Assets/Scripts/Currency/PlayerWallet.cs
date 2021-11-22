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

    public static void LoadSaveData()
    {
        //SetValue(SaveManager.GetCurrency(0), 0);
        SetValue(SaveManager.GetCurrency(1), 1);
        //SetValue(SaveManager.GetCurrency(2), 2);
        //SetValue(SaveManager.GetCurrency(3), 3);
    }

    public static void SetValue (float value, int worldId)
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
