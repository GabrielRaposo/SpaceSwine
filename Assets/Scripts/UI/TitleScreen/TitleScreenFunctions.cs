﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenFunctions : MonoBehaviour
{
    void Start()
    {
        
    }

    public void PlayInput()
    {
        Debug.Log("Play");
        SceneTransition.LoadScene((int) BuildIndex.TestExplorationStage);
    }

    public void OptionsInput()
    {
        Debug.Log("Options");
    }

    public void QuitInput()
    {
        GameManager.QuitGame();
    }
}