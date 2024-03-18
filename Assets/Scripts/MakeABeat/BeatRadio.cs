using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MakeABeat
{
    public class BeatRadio : MonoBehaviour
    {
        [SerializeField] AK.Wwise.Event OnConfirmAKEvent;
        [SerializeField] AK.Wwise.Event OnCancelAKEvent;
        [SerializeField] List<AK.Wwise.Event> noiseAKEvents;
        [SerializeField] List<AK.Wwise.RTPC> noiseRTPCs;
        [SerializeField] DuctTapeLabel labelDisplay;
        [SerializeField] ParticleSystem pulsePS;

        [Header("Visual")]
        [SerializeField] Color mutedColor;
        [SerializeField] SpriteRenderer[] renderers;

        SpriteSwapper screenSwapper;
        TapeBox tapeBox;

        int currentIndex;
        int signatureCount;
        bool isMuted;

        Sequence sequence;
        BeatTrackNavigation navigation;
        BeatMaster beatMaster;

        private void Awake() 
        {
            beatMaster = GetComponentInParent<BeatMaster>();
            navigation = GetComponentInParent<BeatTrackNavigation>();

            if (!beatMaster || !navigation)
            {
                gameObject.SetActive(false);
                return;
            }

            screenSwapper = GetComponentInChildren<SpriteSwapper>();
        }

        void Start()
        {
            beatMaster.SignaturePulse_Action += (f) => 
            {
                if (!pulsePS || isMuted || currentIndex < 0)
                    return;

                pulsePS.Play();
            };
            beatMaster.CyclePulse_Action += CyclePulse; 
            beatMaster.StopAll_Action    += () => 
            {
                gameObject.StopAllEvents();
            };

            UpdateScreenVisualState();
            //UpdatePlayingTapeVisual();

            ChangeStation (-1, silent: true);
        }

        private void UpdateScreenVisualState()
        {
            if (!screenSwapper)
                return;

            screenSwapper.SetSpriteState(currentIndex + 1);
        }


        public void SetSelected (bool value)
        {
            if (!labelDisplay)
                return;

            labelDisplay.SetVisibility( value );
        }

        public void OnConfirmInput()
        {
            int index = currentIndex + 1;
            if (index >= noiseAKEvents.Count)
                index = -1;

            if (beatMaster)
                beatMaster.StartCycle();

            ChangeStation (index);
        }

        public void OnCancelInput()
        {
            ChangeStation (-1);
        }

        private void CyclePulse()
        {
            gameObject.StopAllEvents();
            
            foreach (AK.Wwise.Event noise in noiseAKEvents)
            {
                if (noise == null) 
                    continue;

                noise.Post(gameObject);
            }

            ChangeStation(currentIndex);
        }

        private void UpdateVolumes()
        {
            for (int i = 0; i < noiseRTPCs.Count; i++) 
            {
                int value = i == currentIndex ? 100 : 0;

                if (isMuted)
                    value = 0;

                noiseRTPCs[i].SetGlobalValue(value);
            }
        }

        public void ChangeStation (int index, bool silent = false)
        {
            currentIndex = index;

            UpdateVolumes();

            if (!silent)
            {
                if (currentIndex < 0)
                    OnCancelAKEvent.Post(gameObject);
                else
                    OnConfirmAKEvent.Post(gameObject);
            }

            UpdateScreenVisualState();

            //if (currentEvent != null)
            //    UpdateMutedState();

            if (currentIndex < 0)
                return;

            if (sequence != null)
                sequence.Kill();

            transform.localScale = Vector3.one;

            sequence = DOTween.Sequence();
            sequence.Append( transform.DOPunchScale(Vector3.one * .05f, duration: .2f, vibrato: 0) );

        }

        public void ToggleMute()
        {
            SetMutedState(!isMuted);
        }

        public void SetMutedState(bool value)
        {
            isMuted = value;
            
            foreach (SpriteRenderer r in renderers)
            {
                r.color = value ? mutedColor : Color.white;
            }

            UpdateVolumes();
        }

        private void OnDisable() 
        {
             gameObject.StopAllEvents();
        }
    }
}
