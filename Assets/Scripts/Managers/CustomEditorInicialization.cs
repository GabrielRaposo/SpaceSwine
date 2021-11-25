using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEditorInicialization : MonoBehaviour
{
    static bool initialized;

    public static void Initialize()
    {
        if (initialized)
            return;

        StoryEventSaveConverter.FromSaveToAssets();
        PlayerWallet.LoadSaveData();
        CurrencyInstanceList.Init();

        initialized = true;
    }
}
