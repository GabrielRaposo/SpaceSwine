using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevLocker.Utils;

public class TitleScreenFunctions : MonoBehaviour
{
    [SerializeField] SceneReference newFileScene;

    TitleStateManager titleStateManager;

    private void Awake() 
    {
        titleStateManager = GetComponentInParent<TitleStateManager>();
    }

    public void NewGameInput()
    {
        SaveManager.ResetSave();

        TrailerSceneCaller.AutoStart = true;
        PlaylistPlayer.CutsceneMode = true;

        ContinueInput();
    }

    public void ContinueInput()
    {
        string scenePath = SaveManager.GetSpawnData().scenePath;

        if ( string.IsNullOrEmpty(scenePath) )
            scenePath = newFileScene.ScenePath;

        GameManager.GoToScene (scenePath, saveScenePath: true);
    }

    public void QuitInput()
    {
        GameManager.QuitGame();
    }
}
