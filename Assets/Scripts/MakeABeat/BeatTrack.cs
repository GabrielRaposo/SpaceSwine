using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace MakeABeat
{
    public class BeatTrack : MonoBehaviour
    {
        [SerializeField] BeatTapeScriptableObject beatTapeData;
        [SerializeField] BeatTapeDisplay tapeDisplay;
        [SerializeField] BeatTapeDisplay queuedTapeDisplay;
        [SerializeField] BeatTapeCursor cursor;
        //[SerializeField] SpriteRenderer tapeRenderer;

        Sprite boxPreviewState;
        SpriteSwapper lidSwapper;

        BeatTapeScriptableObject currentBeatTape;
        TapeBox tapeBox;

        int signatureCount;
        bool isRunning;

        bool isSelected;

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
        }

        public void SetSelected (bool value)
        {
            cursor.SetState(value);
            isSelected = value;
        }

        public void SetQueuedState (BeatTapeScriptableObject beatTapeData, TapeBox tapeBox)
        {
            this.tapeBox = tapeBox;
            this.beatTapeData = beatTapeData;
        }

        public void Install ()
        {   
            if (currentBeatTape != null && currentBeatTape != beatTapeData)
                tapeBox.RestoreToAvailables(currentBeatTape);

            if (beatTapeData)
                currentBeatTape = beatTapeData.silent ? null : beatTapeData;
            else 
                currentBeatTape = null;

            if (currentBeatTape == null && beatTapeData == null)
                return;

            RestoreVisualState();

            if (beatTapeData && beatTapeData.silent)
                beatTapeData = null;

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
                tapeBox.RestoreToAvailables(currentBeatTape);
                if (beatTapeData == currentBeatTape)
                {
                    beatTapeData = null;
                    SetQueuedTapeState(null);
                }
                currentBeatTape = null;
                tapeDisplay.SetState(BeatTapeDisplay.State.Off);
            }
            else
            {
                tapeBox.RestoreToAvailables(beatTapeData);
                beatTapeData = null;
                SetQueuedTapeState(null);
            }

            CloseTheLid(false);

        }

        public void CloseTheLid(bool value)
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

        public void SetTapePreviewState (Sprite sprite)
        {
            if (!tapeDisplay)
                return;

            tapeDisplay.SetSprite(sprite);
            tapeDisplay.SetState( sprite == null ? BeatTapeDisplay.State.Off : BeatTapeDisplay.State.Preview );
        }

        public void SetBoxPreviewState(Sprite sprite)
        {
            boxPreviewState = sprite;
        }

        public void SetQueuedTapeState (Sprite sprite)
        {
            if (!queuedTapeDisplay)
                return;

            queuedTapeDisplay.SetSprite( boxPreviewState == null ? sprite : boxPreviewState);
            queuedTapeDisplay.SetState( sprite == null ? BeatTapeDisplay.State.Off : BeatTapeDisplay.State.Preview );
        }

        public void RestoreVisualState()
        {
            if (currentBeatTape == null && beatTapeData == null)
            {
                Debug.Log("A");
                tapeDisplay.SetSprite(null);
                tapeDisplay.SetState(BeatTapeDisplay.State.Off);
                SetQueuedTapeState(null);
                CloseTheLid(false);
                return;
            }

            if (currentBeatTape == beatTapeData)
            {
                Debug.Log("B: " + currentBeatTape.name);
                tapeDisplay.SetSprite(currentBeatTape.frontalSprite);
                tapeDisplay.SetState(BeatTapeDisplay.State.On);
                SetQueuedTapeState(null);
                CloseTheLid(true);
                return;
            }

            if (beatTapeData.silent)
            {
                Debug.Log("C");
                tapeDisplay.SetSprite(currentBeatTape != null ? currentBeatTape.frontalSprite : null);
                tapeDisplay.SetState(currentBeatTape != null ? BeatTapeDisplay.State.On : BeatTapeDisplay.State.Off);
                SetQueuedTapeState(null);
                CloseTheLid(false);
            }
        }

        public bool IsEmpty
        {
            get { return beatTapeData == null; }
        }

        private void OnDisable() 
        {
             gameObject.StopAllEvents();
        }
    }
}
