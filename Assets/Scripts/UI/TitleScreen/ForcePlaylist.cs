using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePlaylist : MonoBehaviour
{
    [SerializeField] PlaylistScriptableObject playlist;

    void Start()
    {
        if (playlist == null)
            return;

        if (SoundtrackManager.Instance)
            SoundtrackManager.Instance.ForceSkipToPlaylist(playlist);
    }
}
