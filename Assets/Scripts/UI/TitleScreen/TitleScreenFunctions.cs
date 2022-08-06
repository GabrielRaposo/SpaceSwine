using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenFunctions : MonoBehaviour
{
    TitleStateManager titleStateManager;

    private void Awake() 
    {
        titleStateManager = GetComponentInParent<TitleStateManager>();
    }

    void Start()
    {
        if (!SaveManager.Initiated)
            SaveManager.Load();
    }

    public void PlayInput()
    {
        //Debug.Log("Play");

        TrailerSceneCaller.AutoStart = true;
        SceneTransition.LoadScene((int)BuildIndex.Ship, SceneTransition.TransitionType.WhiteFade);
        //SceneTransition.LoadScene((int) BuildIndex.World1Exploration, SceneTransition.TransitionType.WhiteFade );
    }

    //public void OptionsInput()
    //{
    //    //Debug.Log("Options");

    //    TitleStateManager titleStateManager = GetComponentInParent<TitleStateManager>();
    //    if (titleStateManager)
    //    {
    //        titleStateManager.SetOptionsState();
    //    }
    //}

    //public void ChillInput()
    //{
    //    //Debug.Log("Options");

    //    TitleStateManager titleStateManager = GetComponentInParent<TitleStateManager>();
    //    if (titleStateManager)
    //    {
    //        titleStateManager.SetChillState();
    //    }
    //}

    public void QuitInput()
    {
        GameManager.QuitGame();
    }
}
