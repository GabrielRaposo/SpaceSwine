using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevLocker.Utils;

public class SetSpawnDataOnStart : MonoBehaviour
{
    [SerializeField] SceneReference sceneData;
    [SerializeField] int spawnIndex = -1;

    void Start()
    {
        if (!SaveManager.Initiated)
            return;

        if (sceneData != null)
        {
            SaveManager.SetSpawnPath (sceneData.ScenePath);
        }

        if (spawnIndex > -1)
        {
            SaveManager.SetSpawnIndex (spawnIndex);
        }
    }
}
