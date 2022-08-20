using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpawnPointOnStart : MonoBehaviour
{
    public int index;

    void Awake()
    {
        //Debug.Log("-- ChangeSpawnPointOnStart");
        SpawnManager.Index = index;
    }
}
