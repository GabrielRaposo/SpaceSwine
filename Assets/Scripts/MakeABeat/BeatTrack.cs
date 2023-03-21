using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace MakeABeat
{
    public class BeatTrack : MonoBehaviour
    {
        [SerializeField] BeatTapeScriptableObject queuedBeatTape;
        [SerializeField] BeatTapeDisplay tapeDisplay;
        [SerializeField] BeatTapeDisplay queuedTapeDisplay;
        [SerializeField] BeatTapeCursor cursor;
        [SerializeField] DuctTapeLabel labelDisplay;

        [Header("Audio")]
        [SerializeField] AK.Wwise.Event instalationAKEvent;
        [SerializeField] AK.Wwise.Event unnistallAKEvent;
        [SerializeField] AK.Wwise.Event enqueueAKEvent;
        [SerializeField] AK.Wwise.Event removeFromQueueAKEvent;

        [Header("Visual")]
        [SerializeField] Color mutedColor;
        [SerializeField] SpriteRenderer[] renderers;

        SpriteSwapper lidSwapper;

        BeatTapeScriptableObject currentBeatTape;
        TapeBox tapeBox;

        int signatureCount;
        bool isMuted;

        Sequence sequence;
        BeatTrackNavigation navigation;
        BeatMaster beatMaster;

        private void Awake() 
        {
            lidSwapper = GetComponentInChildren<SpriteSwapper>();
            
            beatMaster = GetComponentInParent<BeatMaster>();
            navigation = GetComponentInParent<BeatTrackNavigation>();
            if (!beatMaster || !navigation)
            {
                gameObject.SetActive(false);
                return;
            }
        }

        void Start()
        {
            beatMaster.CyclePulse_Action += CyclePulse; 
            beatMaster.StopAll_Action    += () => 
            {
                gameObject.StopAllEvents();
            };

            UpdateQueuedTapeVisual();
            UpdatePlayingTapeVisual();
        }

        public void SetSelected (bool value)
        {
            if (!labelDisplay)
                return;

            labelDisplay.SetVisibility( value );
        }

        public void EnqueueTape (BeatTapeScriptableObject beatTapeData, TapeBox tapeBox)
        {
            this.tapeBox = tapeBox;

            // Se já tem uma beatTapeData e ela não está tocando no loop
            if (queuedBeatTape != null && queuedBeatTape != currentBeatTape)
                tapeBox.RestoreToAvailables(queuedBeatTape);

            if (currentBeatTape == null && beatTapeData.silent)
                queuedBeatTape = null;
            else
            {
                if (enqueueAKEvent != null)
                    enqueueAKEvent.Post(gameObject);

                queuedBeatTape = beatTapeData;
            }

            UpdateQueuedTapeVisual();
            CloseTheLid(queuedBeatTape == null && !beatTapeData.silent);

            if (beatMaster && beatMaster.StartCycle())
                Install();
        }

        public void Install ()
        {
            if (currentBeatTape != queuedBeatTape)
            {

                if (currentBeatTape != null && tapeBox) 
                {
                    if (unnistallAKEvent != null)
                        unnistallAKEvent.Post(gameObject);
                    tapeBox.RestoreToAvailables(currentBeatTape);
                }
                else
                {
                    if (instalationAKEvent != null)
                        instalationAKEvent.Post(gameObject);
                }
            }

            if (queuedBeatTape && queuedBeatTape.silent)
                queuedBeatTape = null;

            currentBeatTape = queuedBeatTape;

            UpdateQueuedTapeVisual();
            UpdatePlayingTapeVisual();

            if (currentBeatTape != null)
                UpdateMutedState();

            if (currentBeatTape == null && queuedBeatTape == null)
                return;

            if (sequence != null)
                sequence.Kill();

            sequence = DOTween.Sequence();
            sequence.Append( transform.DOPunchScale(Vector3.one * .05f, duration: .2f, vibrato: 0) );

        }

        public void InstantUninstall(TapeBox tapeBox)
        {
            if (currentBeatTape != null && currentBeatTape.sampleAKEvent != null)
            {
                gameObject.StopAllEvents();
            }

            if (currentBeatTape != null)
            {
                if (tapeBox)
                    tapeBox.RestoreToAvailables(currentBeatTape);
                
                if (currentBeatTape == queuedBeatTape)
                    queuedBeatTape = null;
                currentBeatTape = null;

                if (unnistallAKEvent != null)
                    unnistallAKEvent.Post(gameObject);

                UpdatePlayingTapeVisual();
                UpdateQueuedTapeVisual();
            }
            else if (queuedBeatTape != null)
            {
                if (tapeBox)
                    tapeBox.RestoreToAvailables(queuedBeatTape);

                if (removeFromQueueAKEvent != null)
                    removeFromQueueAKEvent.Post(gameObject);

                queuedBeatTape = null;
                UpdateQueuedTapeVisual();
            }
        }

        public void CloseTheLid (bool value)
        {
            if (!lidSwapper)
                return;

            lidSwapper.SetSpriteState(value ? 1 : 0);   
        }

        private void CyclePulse()
        {
            // -- Se já tem uma Tape tocando e ela não terminou ainda, espera ela acabar antes de trocar
            if (currentBeatTape != null)
            {
                signatureCount++;

                if (signatureCount > currentBeatTape.signatureDuration - 1)
                    signatureCount = 0;
                else return;
            }

            if (currentBeatTape != null && currentBeatTape.sampleAKEvent != null)
            {
                gameObject.StopAllEvents();
            }

            Install();

            if (currentBeatTape == null)
                return;

            if (currentBeatTape.sampleAKEvent != null)
                currentBeatTape.sampleAKEvent.Post(gameObject);
        }

        private void UpdateQueuedTapeVisual()
        {
            if (queuedBeatTape == null || queuedBeatTape == currentBeatTape)
            {
                queuedTapeDisplay.SetSprite(null);
                queuedTapeDisplay.SetState(BeatTapeDisplay.State.Off);
                return;
            }

            if (queuedBeatTape != null)
            {
                queuedTapeDisplay.SetSprite(queuedBeatTape.frontalSprite);
                queuedTapeDisplay.SetState(BeatTapeDisplay.State.Preview);
            }
        }

        private void UpdatePlayingTapeVisual()
        {
            CloseTheLid (currentBeatTape != null && currentBeatTape == queuedBeatTape);

            if (currentBeatTape == null)
            {
                tapeDisplay.SetSprite(null);
                tapeDisplay.SetState(BeatTapeDisplay.State.Off);
                
                if (labelDisplay)
                    labelDisplay.Hide();
            }
            else
            {
                tapeDisplay.SetSprite(currentBeatTape.frontalSprite);
                tapeDisplay.SetState(BeatTapeDisplay.State.On);

                if (labelDisplay)
                {
                    labelDisplay.SetText(currentBeatTape.title);
                    if (this == navigation.GetSelectedTrack())
                        labelDisplay.Show();
                }
            }
        }

        public void ToggleMute()
        {
            SetMutedState (!isMuted);
        }

        public void SetMutedState(bool value)
        {
            isMuted = value;
            UpdateMutedState();
        }

        private void UpdateMutedState()
        {
            foreach (SpriteRenderer r in renderers)
            {
                r.color = isMuted ? mutedColor : Color.white;
            }

            if (!currentBeatTape || currentBeatTape.volumeAKParam == null)
                return;

            currentBeatTape.volumeAKParam.SetGlobalValue( isMuted ? 0 : 100 );
        }

        private void OnDisable() 
        {
             gameObject.StopAllEvents();
        }
    }
}
