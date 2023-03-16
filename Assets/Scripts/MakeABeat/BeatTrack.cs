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
        [SerializeField] BeatTapeCursor cursor;
        //[SerializeField] SpriteRenderer tapeRenderer;

        SpriteSwapper lidSwapper;

        BeatTapeScriptableObject currentBeatTape;

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

        public void Install (BeatTapeScriptableObject beatTapeData, TapeBox tapeBox)
        {
            // if one is playing, add to tape queue
            if (this.beatTapeData != null)
                tapeBox.RestoreToAvailables(this.beatTapeData);

            this.beatTapeData = beatTapeData;

            RestoreVisualState();

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

            tapeBox.RestoreToAvailables(beatTapeData);

            beatTapeData = null;
            currentBeatTape = null;

            SetLidState(false);
            tapeDisplay.SetState(BeatTapeDisplay.State.Off);

        }

        public void SetLidState(bool value)
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

            currentBeatTape = beatTapeData;

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

        public void RestoreVisualState()
        {
            if (beatTapeData != null)
            {
                tapeDisplay.SetSprite(beatTapeData.frontalSprite);
                tapeDisplay.SetState(BeatTapeDisplay.State.On);

                SetLidState(true);
            }
            else
            {
                tapeDisplay.SetState(BeatTapeDisplay.State.Off);

                SetLidState(false);
            }
        }

        private void OnDisable() 
        {
             gameObject.StopAllEvents();
        }
    }
}
