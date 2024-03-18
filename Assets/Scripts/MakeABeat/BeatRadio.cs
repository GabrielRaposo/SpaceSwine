using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeABeat
{
    public class BeatRadio : MonoBehaviour
    {
        [SerializeField] List<AK.Wwise.Event> noiseAKEvents;
        [SerializeField] DuctTapeLabel labelDisplay;
        [SerializeField] ParticleSystem pulsePS;

        AK.Wwise.Event currentEvent;

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
                if (!pulsePS || isMuted || currentEvent == null)
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
        }

        public void SetSelected (bool value)
        {
            if (!labelDisplay)
                return;

            labelDisplay.SetVisibility( value );
        }

        public void OnConfirmInput()
        {
            Install( (currentIndex + 1) % noiseAKEvents.Count );
        }

        public void OnCancelInput()
        {
            Install (0);
        }

        private void CyclePulse()
        {
            if (currentEvent != null)
            {
                gameObject.StopAllEvents();
            }

            Install(currentIndex);

            if (currentEvent == null)
                return;

            currentEvent.Post(gameObject);
        }

        public void Install (int index)
        {
            currentIndex = index;
            currentEvent = index == 0 ? null : noiseAKEvents[index % noiseAKEvents.Count];
            Debug.Log($"currentIndex: {currentIndex}");

            //UpdateQueuedTapeVisual();
            //UpdatePlayingTapeVisual();

            //if (currentEvent != null)
            //    UpdateMutedState();

            if (currentEvent == null)
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
