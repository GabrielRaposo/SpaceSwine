using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeABeat
{
    public class BeatRadio : MonoBehaviour
    {
        [SerializeField] List<AK.Wwise.Event> noiseAKEvents;
        [SerializeField] List<AK.Wwise.RTPC> noiseRTPCs;
        [SerializeField] DuctTapeLabel labelDisplay;
        [SerializeField] ParticleSystem pulsePS;

        SpriteSwapper lidSwapper;
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

            lidSwapper = GetComponentInChildren<SpriteSwapper>();
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

            //UpdateQueuedTapeVisual();
            //UpdatePlayingTapeVisual();

            Install (-1);
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
            Install (index);
        }

        public void OnCancelInput()
        {
            Install (-1);
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

            Install(currentIndex);
        }

        public void Install (int index)
        {
            for (int i = 0; i < noiseRTPCs.Count; i++) 
            {
                noiseRTPCs[i].SetGlobalValue(i == index ? 100 : 0);
            }

            currentIndex = index;
            //Debug.Log($"currentIndex: {currentIndex}");

            //UpdateQueuedTapeVisual();
            //UpdatePlayingTapeVisual();

            //if (currentEvent != null)
            //    UpdateMutedState();

            if (currentIndex < 0)
                return;

            if (sequence != null)
                sequence.Kill();

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
            //UpdateMutedState();
        }

        //private void UpdateMutedState()
        //{
        //    foreach (SpriteRenderer r in renderers)
        //    {
        //        r.color = isMuted ? mutedColor : Color.white;
        //    }

        //    if (!currentBeatTape || currentBeatTape.volumeAKParam == null)
        //        return;

        //    currentBeatTape.volumeAKParam.SetGlobalValue( isMuted ? 0 : 100 );
        //}

        private void OnDisable() 
        {
             gameObject.StopAllEvents();
        }
    }
}
