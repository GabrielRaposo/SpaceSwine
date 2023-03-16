using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace MakeABeat
{
    public class TapeBox : MonoBehaviour
    {
        [SerializeField] float shownY;
        [SerializeField] float hiddenY;
        [SerializeField] float duration;
        [SerializeField] float itemSpacing;
        
        [Header("References")]
        [SerializeField] List<BeatTapeScriptableObject> beatTapes;
        [SerializeField] Transform cursor;
        [SerializeField] GameObject boxTapePrefab;
        [SerializeField] TextMeshPro labelDisplay;

        bool isShown;
        int current;

        BeatTrack selectedTrack;
        List<TapeBoxItem> items;

        Sequence sequence;

        void Start()
        {
            SetupTapes();
            SetShownPosition(false);
            UpdateSelected(-1);
        }

        private void SetupTapes()
        {
            items = new List<TapeBoxItem>();

            if (!boxTapePrefab || beatTapes.Count < 1)
                return;

            for (int i = 0; i < beatTapes.Count; i++)
            {
                Vector2 spawnPos = boxTapePrefab.transform.position + (Vector3.right * itemSpacing * i);
                GameObject tapeObject = Instantiate( boxTapePrefab, spawnPos, Quaternion.identity, transform);

                TapeBoxItem tape = tapeObject.GetComponent<TapeBoxItem>();
                if (tape)
                {
                    tape.Setup(beatTapes[i]);
                    items.Add(tape);
                }

            }

            boxTapePrefab.SetActive(false);
        }

        private void SetShownPosition (bool value)
        {
            transform.position = new Vector2(transform.position.x, value ? shownY : hiddenY);
            isShown = value;
        }

        public void Show (BeatTrack selectedTrack, bool value)
        {
            if (sequence != null)
                sequence.Kill();

            {
                this.selectedTrack = selectedTrack;

                if (value)
                    BeatMenuController.Focus = MakeABeatFocus.Box;
                else
                    BeatMenuController.Focus = MakeABeatFocus.Tapes;

                UpdateSelected( value ? current : -1 );
            }

            sequence = DOTween.Sequence();
            sequence.Append( transform.DOLocalMoveY(value ? shownY : hiddenY, duration).SetEase(Ease.OutBounce) );

            sequence.OnComplete( () => 
            {
                SetShownPosition(value);
            });
        }

        public void UpdateSelected (int index)
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].SetHighlighted( i == index );
            }
            
            if (!cursor)
                return;

            bool value = index > -1;
            cursor.gameObject.SetActive( value );
            
            if (value)
            {
                TapeBoxItem item = items[index % items.Count];
                cursor.SetParent(item.transform);
                cursor.position = item.transform.position + (Vector3.up * .5f);
            
                if (labelDisplay)
                    labelDisplay.text = item.BeatTape.title;

                if (selectedTrack)
                    selectedTrack.SetTapePreviewState(item.BeatTape.frontalSprite);
            }
            else
            {
                if (labelDisplay)
                    labelDisplay.text = string.Empty;
            }
            
        }

        public void MoveCursor (int direction)
        {
            current += direction;
            if (current < 0)
                current = items.Count - 1;
            current %= items.Count;

            UpdateSelected(current);
        }

        public void OnSelectInput()
        {
            if (selectedTrack == null)
                return;

            TapeBoxItem item = items[current % items.Count];
            selectedTrack.Install( item.BeatTape );

            Show (null, false);
            BeatMenuController.Focus = MakeABeatFocus.Tapes;
        }
    }
}
