using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace MakeABeat
{
    public class BeatTrack : MonoBehaviour
    {
        [SerializeField] BeatTapeScriptableObject queuedBeatTape;
        [SerializeField] BeatTapeDisplay tapeDisplay;
        [SerializeField] BeatTapeDisplay queuedTapeDisplay;
        [SerializeField] BeatTapeCursor cursor;

        Sprite boxPreviewState;
        SpriteSwapper lidSwapper;

        BeatTapeScriptableObject currentBeatTape;
        TapeBox tapeBox;

        int signatureCount;
        bool isRunning;

        Sequence sequence;

        private void Awake() 
        {
            lidSwapper = GetComponentInChildren<SpriteSwapper>();
        }

        void Start()
        {
            BeatMaster beatMaster = GetComponentInParent<BeatMaster>();
            if (!beatMaster)
            {
                gameObject.SetActive(false);
                return;
            }

            beatMaster.CyclePulse_Action += CyclePulse; 

            UpdateQueuedTapeVisual();
            UpdatePlayingTapeVisual();
        }

        public void SetSelected (bool value)
        {
            cursor.SetState(value);
            //isSelected = value;
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
        }

        public void Install ()
        {   
            if (currentBeatTape != null && currentBeatTape != queuedBeatTape && tapeBox)
                tapeBox.RestoreToAvailables(currentBeatTape);

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

        public void SetArrowsVisibility (bool value)
        {
            if (!cursor)
                return;

            cursor.SetArrowsVisibility(value);
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
            CloseTheLid (currentBeatTape != null);

            if (currentBeatTape == null)
            {
                tapeDisplay.SetSprite(null);
                tapeDisplay.SetState(BeatTapeDisplay.State.Off);
            }
            else
            {
                tapeDisplay.SetSprite(currentBeatTape.frontalSprite);
                tapeDisplay.SetState(BeatTapeDisplay.State.On);
            }
        }

        private void OnDisable() 
        {
             gameObject.StopAllEvents();
        }
    }
}
