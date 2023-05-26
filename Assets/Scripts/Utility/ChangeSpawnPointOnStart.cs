using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpawnPointOnStart : MonoBehaviour
{
    public int index;

    void Awake()
    {
        StartCoroutine( WaitForSaveInitiation() );
    }

    IEnumerator WaitForSaveInitiation()
    {
        yield return new WaitUntil ( () => SaveManager.Initiated );

        SaveManager.SetSpawnIndex(index);
    }
}
