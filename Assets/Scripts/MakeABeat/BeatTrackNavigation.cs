using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeABeat
{
    public class BeatTrackNavigation : MonoBehaviour
    {
        [SerializeField] BeatTrack initialSelection;

        BeatTrack[] tracks;
        BeatTrack CurrentTrack;

        void Start()
        {
            tracks = GetComponentsInChildren<BeatTrack>();
            foreach(BeatTrack track in tracks)
            {
                BeatNavigationItem navigationItem = track.GetComponent<BeatNavigationItem>();
                if (navigationItem)
                    navigationItem.Setup();

                track.SetSelected(false);
            }

            CurrentTrack = initialSelection;
            UpdateSelection();
        }

        private void UpdateSelection()
        {
            for (int i = 0; i < tracks.Length; i++)
            {
                BeatTrack track = tracks[ i % tracks.Length ];
                track.SetSelected( track == CurrentTrack ); 
            }
        }

        public void MoveCursor (Vector2 direction)
        {    
            BeatNavigationItem navigationItem = CurrentTrack.GetComponent<BeatNavigationItem>();
            if (!navigationItem)
                return;

            BeatNavigationItem output = navigationItem.FindItemOnDirection(direction);
            if (output == null)
                return;

            CurrentTrack = output.GetComponent<BeatTrack>();
            UpdateSelection();
        }

        public BeatTrack GetSelectedTrack()
        {
            return CurrentTrack;
        }

        public void SetArrowsVisibility(bool value)
        {
            if (!CurrentTrack)
                return;

            CurrentTrack.SetArrowsVisibility(value);
        }

        public void OnConfirmInput() 
        {
            SetArrowsVisibility(false);
        }

        public void OnCancelInput(TapeBox tapeBox)
        {
            if (!CurrentTrack)
                return;

            CurrentTrack.InstantUninstall(tapeBox);
        }
    }
}
