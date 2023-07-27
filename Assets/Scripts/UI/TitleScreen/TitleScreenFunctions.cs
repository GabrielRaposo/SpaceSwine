using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevLocker.Utils;
using MakeABeat;

public class TitleScreenFunctions : MonoBehaviour
{
    [SerializeField] SceneReference newFileScene;
    [SerializeField] SceneReference makeABeatScene;

    TitleStateManager titleStateManager;

    private void Awake() 
    {
        titleStateManager = GetComponentInParent<TitleStateManager>();
    }

    private void Start() 
    {
        BeatMenuController.ExitToTitle = false;
    }

    public void NewGameInput()
    {
        SaveManager.ResetSave();
        DebugDisplay.Call ("New Game: Save Reset.");

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

    public void CallMakeABeat()
    {
        if (makeABeatScene == null)
            return;

        SoundtrackManager soundtrackManager = SoundtrackManager.Instance;
        if (soundtrackManager)
            soundtrackManager.FadeOutMusic(1f);

        BeatMenuController.ExitToTitle = true;
        SoundtrackManager.OverrideChecksTrigger = true;

        GameManager.GoToScene (makeABeatScene.ScenePath);
    }

    public void QuitInput()
    {
        GameManager.QuitGame();
    }
}
