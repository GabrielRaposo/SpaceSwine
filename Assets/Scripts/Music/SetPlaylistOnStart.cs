using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlaylistOnStart : MonoBehaviour
{
    [SerializeField] PlaylistScriptableObject playlist;

    public static bool Block = true;

    void Start()
    {
        if (Block)
        {
            Block = false;
            return;
        }

        if (playlist == null)
            return;

        if (SoundtrackManager.Instance)
            SoundtrackManager.Instance.SetPlaylist(playlist);
    }
}
