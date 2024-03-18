using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetShuttleSpawnScenePath : MonoBehaviour
{
    void Start()
    {
        StartCoroutine ( WaitForSave() );
    }

    IEnumerator WaitForSave()
    {
        yield return new WaitUntil( () => SaveManager.Initiated );

        SaveManager.ShuttleExitLocationPath = SceneManager.GetActiveScene().path;
        SaveManager.SetSpawnPath( SceneManager.GetActiveScene().path );
    }
}