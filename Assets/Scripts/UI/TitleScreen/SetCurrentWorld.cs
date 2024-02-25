using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCurrentWorld : MonoBehaviour
{
    [SerializeField] int world;

    void Start()
    {
        StartCoroutine( WaitForSaveInitiation() );
    }

    IEnumerator WaitForSaveInitiation()
    {
        if (!SaveManager.Initiated) 
            yield return new WaitUntil ( () => SaveManager.Initiated);

        int indexedWorld = world - 1;

        SaveManager.CurrentWorld = indexedWorld;

        if (SoundtrackManager.Instance && PlaylistReferences.Instance)
            SoundtrackManager.Instance.ForceSkipToPlaylist( PlaylistReferences.Instance.GetPlaylistBy(indexedWorld) );
    }
}
