using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlaylistAccordingToProgress : StoryEventDependent
{
    [Header("Criterias")]
    [SerializeField] StoryEventScriptableObject world1UnlockEvent;
    [SerializeField] StoryEventScriptableObject world2UnlockEvent;
    [SerializeField] StoryEventScriptableObject world3UnlockEvent;

    [Header("Playlists")]
    [SerializeField] PlaylistScriptableObject startingPlaylist;
    [SerializeField] PlaylistScriptableObject world1Playlist;
    [SerializeField] PlaylistScriptableObject world2Playlist;
    [SerializeField] PlaylistScriptableObject world3Playlist;

    PlaylistScriptableObject customPlaylist;

    void Start()
    {
        CallDependentAction
        (
            action: () => StartCoroutine (WaitForSaveInitiation()),
            extraFrames: 1
        );
    }

    IEnumerator WaitForSaveInitiation() 
    {
        yield return new WaitUntil ( () => SaveManager.Initiated );

        customPlaylist = new PlaylistScriptableObject();
        customPlaylist.shuffle = true;
        customPlaylist.list = new TrackList();

        AddToCustomPlaylist(startingPlaylist);

        if (world1UnlockEvent && StoryEventsManager.IsComplete(world1UnlockEvent))
            AddToCustomPlaylist(world1Playlist);

        if (world2UnlockEvent && StoryEventsManager.IsComplete(world2UnlockEvent))
            AddToCustomPlaylist(world2Playlist);

        if (world3UnlockEvent && StoryEventsManager.IsComplete(world3UnlockEvent))
            AddToCustomPlaylist(world3Playlist);

        if (SoundtrackManager.Instance)
            SoundtrackManager.Instance.ForceSkipToPlaylist(customPlaylist);
    }

    private void AddToCustomPlaylist (PlaylistScriptableObject playlist)
    {
        if (!playlist)
            return;

        foreach (var track in playlist.list)
        {
            if (customPlaylist.list.Contains(track))
                continue;

            customPlaylist.list.Add(track);
        }
    }
}
