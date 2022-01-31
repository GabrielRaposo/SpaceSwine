using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpawnPointOnStart : MonoBehaviour
{
    public int index;

    void Start()
    {
        SpawnManager.index = index;
    }
}
