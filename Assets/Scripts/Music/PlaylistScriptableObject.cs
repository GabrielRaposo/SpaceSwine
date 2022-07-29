using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee.List;

[CreateAssetMenu (fileName = "Playlist", menuName = "ScriptableObjects/Playlist Data") ]
public class PlaylistScriptableObject : ScriptableObject
{
	//[Reorderable(paginate = true, pageSize = 2)]
	[Reorderable] public TrackList list;
}

[System.Serializable]
public class TrackList : ReorderableArray<MusicDataScriptableObject> {
}