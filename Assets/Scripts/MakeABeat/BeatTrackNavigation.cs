using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeABeat
{
    public class BeatTrackNavigation : MonoBehaviour
    {
        BeatTrack[] tracks;
        int index;

        void Start()
        {
            tracks = GetComponentsInChildren<BeatTrack>();
            foreach(BeatTrack track in tracks)
                track.SetSelected(false);

            index = 0;
            UpdateSelection();
        }

        private void UpdateSelection()
        {
            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[ i % tracks.Length ].SetSelected( i == index );
            }
        }

        public void MoveCursor(int value)
        {
            index += value;

            if (index < 0)
                index = tracks.Length - 1;
            else 
                index %= tracks.Length;

            UpdateSelection();
        }
    }
}
