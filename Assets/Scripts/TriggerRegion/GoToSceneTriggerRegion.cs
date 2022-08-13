using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToSceneTriggerRegion : MonoBehaviour
{
    [SerializeField] BuildIndex targetScene;
    [SerializeField] int nextSceneSpawnPoint = -1;
    [SerializeField] PlaylistScriptableObject nextScenePlaylist;
    bool activated;

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (!collision.CompareTag("Player"))
            return;    

        if (activated)
            return;

        if (nextSceneSpawnPoint > -1)
            SpawnManager.Index = nextSceneSpawnPoint;
        
        // -- TO-DO: gameManager.SetPlaylistOnStart

        GameManager.GoToScene(targetScene);
        activated = true;   
    }
}
