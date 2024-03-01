using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaylistReferences : MonoBehaviour
{
    public PlaylistScriptableObject fullPlaylist;
    public PlaylistScriptableObject world1Playlist;
    public PlaylistScriptableObject world2Playlist;
    public PlaylistScriptableObject world3Playlist;

    static public PlaylistReferences Instance;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
    }

    public PlaylistScriptableObject GetPlaylistBy (int index)
    {
        switch (index) 
        {
            default: return fullPlaylist;
            case 0:  return world1Playlist;
            case 1:  return world2Playlist;
            case 2:  return world3Playlist;
        }
    }
}
