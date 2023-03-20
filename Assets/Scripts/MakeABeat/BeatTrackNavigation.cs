using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeABeat
{
    public class BeatTrackNavigation : MonoBehaviour
    {
        [SerializeField] BeatNavigationItem initialSelection;

        BeatNavigationItem[] items;

        BeatNavigationItem CurrentSelection;

        void Start()
        {
            items = GetComponentsInChildren<BeatNavigationItem>();
            foreach (BeatNavigationItem item in items)
            {
                item.Setup();

                item.SetSelected(false);

                BeatTrack track = item.GetComponent<BeatTrack>();
                if (track)
                    track.SetSelected(false);
            }

            CurrentSelection = initialSelection;

            UpdateSelection();
        }

        private void UpdateSelection()
        {
            for (int i = 0; i < items.Length; i++)
            {
                BeatNavigationItem item = items[ i % items.Length ];
                bool isSelected = item == CurrentSelection;

                item.SetSelected( isSelected );

                BeatTrack track = item.GetComponent<BeatTrack>();
                if (track)
                    track.SetSelected( isSelected ); 
            }
        }

        public void MoveCursor (Vector2 direction)
        {    
            if (CurrentSelection == null)
                return;

            BeatNavigationItem output = CurrentSelection.FindItemOnDirection(direction);
            if (output == null)
                return;

            CurrentSelection = output;
            UpdateSelection();
        }

        public BeatTrack GetSelectedTrack()
        {
            return CurrentSelection.GetComponent<BeatTrack>();
        }

        public void SetArrowsVisibility(bool value)
        {
            if (!CurrentSelection)
                return;

            CurrentSelection.SetArrowsVisibility(value);
        }

        public void OnConfirmInput(TapeBox tapeBox) 
        {
            BeatTrack selectedTrack = GetSelectedTrack();
            if (selectedTrack)
            {
                SetArrowsVisibility(false);
                tapeBox.Show(selectedTrack, true);
                BeatMenuController.Focus = MakeABeatFocus.Box;
                return;
            }

            BeatMetronome metronome = CurrentSelection.GetComponent<BeatMetronome>();
            if (metronome)
            {
                metronome.OnConfirmInput();
                return;
            }
        }

        public void OnCancelInput(TapeBox tapeBox)
        {
            if (!CurrentSelection)
                return;

            BeatTrack track = CurrentSelection.GetComponent<BeatTrack>();
            if (track)
                track.InstantUninstall(tapeBox);
        }
    }
}
