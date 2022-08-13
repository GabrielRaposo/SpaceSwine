using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee.List;

[CreateAssetMenu (fileName = "Playlist", menuName = "ScriptableObjects/Playlist Data") ]
public class PlaylistScriptableObject : ScriptableObject
{
    public bool shuffle;

	//[Reorderable(paginate = true, pageSize = 2)]
	[Reorderable] public TrackList list;

	public MusicDataScriptableObject this[int i]
    {
        get { return list[i]; }
    }

    public int Count
    {
        get { return list.Count; }
    }
}

[System.Serializable]
public class TrackList : ReorderableArray<MusicDataScriptableObject> {
}