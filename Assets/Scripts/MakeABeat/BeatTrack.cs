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

        Sprite boxPreviewState;
        SpriteSwapper lidSwapper;

        BeatTapeScriptableObject currentBeatTape;
        TapeBox tapeBox;

        int signatureCount;
        bool isRunning;

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
                beatTapeData = null;

            this.queuedBeatTape = beatTapeData;
            UpdateQueuedTapeVisual();
            CloseTheLid(queuedBeatTape == null);

            if (beatMaster && beatMaster.StartCycle())
                Install();
        }

        public void Install ()
        {
            if (currentBeatTape != null && currentBeatTape != queuedBeatTape && tapeBox) 
            {
                tapeBox.RestoreToAvailables(currentBeatTape);
            }

            if (queuedBeatTape && queuedBeatTape.silent)
                queuedBeatTape = null;

            currentBeatTape = queuedBeatTape;

            UpdateQueuedTapeVisual();
            UpdatePlayingTapeVisual();

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

                UpdatePlayingTapeVisual();
                UpdateQueuedTapeVisual();

            }
            else if (queuedBeatTape != null)
            {
                if (tapeBox)
                    tapeBox.RestoreToAvailables(queuedBeatTape);

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

        private void OnDisable() 
        {
             gameObject.StopAllEvents();
        }
    }
}
