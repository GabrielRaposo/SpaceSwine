﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeABeat
{
    public class BeatTrackNavigation : MonoBehaviour
    {
        [SerializeField] BeatNavigationItem initialSelection;
        [SerializeField] AK.Wwise.Event navigationAKEvent;
        [SerializeField] AK.Wwise.Event onSelectAKEvent;

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
                {
                    track.SetSelected( isSelected ); 
                    continue;
                }

                BeatToy beatToy = item.GetComponent<BeatToy>();
                if (beatToy)
                {
                    beatToy.Show( isSelected );
                    continue;
                }
            }
        }

        public void MoveCursor (Vector2 direction)
        {    
            if (CurrentSelection == null)
                return;

            BeatNavigationItem output = CurrentSelection.FindItemOnDirection(direction);
            if (output == null)
                return;

            if (navigationAKEvent != null)
                navigationAKEvent.Post(gameObject);

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
                if (onSelectAKEvent != null)
                    onSelectAKEvent.Post(gameObject);

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

            BeatToy beatToy = CurrentSelection.GetComponent<BeatToy>();
            if (beatToy)
            {
                beatToy.CallHitSound(1);
                return;
            }
        }

        public void OnCancelInput(TapeBox tapeBox)
        {
            if (!CurrentSelection)
                return;

            BeatTrack track = CurrentSelection.GetComponent<BeatTrack>();
            if (track)
            {
                track.InstantUninstall(tapeBox);
                return;
            }

            BeatToy beatToy = CurrentSelection.GetComponent<BeatToy>();
            if (beatToy)
            {
                beatToy.CallHitSound(2);
                return;
            }
        }

        public void OnOtherInput()
        {
            if (!CurrentSelection)
                return;

            BeatTrack track = CurrentSelection.GetComponent<BeatTrack>();
            if (track) 
            { 
                track.ToggleMute();
                return;
            }

            BeatMetronome metronome = CurrentSelection.GetComponent<BeatMetronome>();
            if (metronome)
            {
                metronome.ToggleMute();
                return;
            }

            BeatToy beatToy = CurrentSelection.GetComponent<BeatToy>();
            if (beatToy)
            {
                beatToy.CallHitSound(3);
                return;
            }
        }

        public void SetAllMutedStates(bool value)
        {
            foreach (BeatNavigationItem item in items)
            {
                BeatTrack track = item.GetComponent<BeatTrack>();
                if (!track)
                    continue;

                track.SetMutedState(value);
            }
        }
    }
}
