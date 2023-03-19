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
        [SerializeField] Transform cursor;
        [SerializeField] GameObject boxTapePrefab;
        [SerializeField] TextMeshPro labelDisplay;
        [SerializeField] SpriteRenderer tapePreviewDisplay;
        [SerializeField] List<BeatTapeScriptableObject> beatTapes;

        bool isShown;
        int current;

        BeatTrack selectedTrack;
        List<TapeBoxItem> items;
        List<TapeBoxItem> availableItems;

        Sequence sequence;

        void Start()
        {
            SetupTapes();
            SetupAvailableTapes();
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

        private void SetupAvailableTapes()
        {
            // TO-DO: Adicionar interção com fitas desbloqueadas
            
            availableItems = new List<TapeBoxItem>();

            foreach (TapeBoxItem item in items)
                availableItems.Add(item);
        }

        private void UpdateAvailables()
        {
            List<TapeBoxItem> aux = new List<TapeBoxItem>();
            foreach (TapeBoxItem item in items)
            {
                if (availableItems.Contains(item))
                    aux.Add(item);
            }
            availableItems = aux;

            foreach (TapeBoxItem item in items)
            {
                item.SetHighlighted(false);
                item.gameObject.SetActive( availableItems.Contains(item) );
            }
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
                //if (this.selectedTrack)
                //    this.selectedTrack.SetQueuedTapeState(null);

                this.selectedTrack = selectedTrack;

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
            for (int i = 0; i < availableItems.Count; i++)
            {
                availableItems[i].SetHighlighted( i == index );
            }

            bool value = index > -1;

            if (value)
            {
                TapeBoxItem item = availableItems[index % availableItems.Count];

                if (labelDisplay)
                    labelDisplay.text = item.BeatTape.title;

                if (tapePreviewDisplay)
                    tapePreviewDisplay.sprite = item.BeatTape.frontalSprite;
            }
            else
            {
                if (labelDisplay)
                    labelDisplay.text = string.Empty;

                if (tapePreviewDisplay)
                    tapePreviewDisplay.sprite = null;
            }
        }

        public void MoveCursor (int direction)
        {
            if (availableItems.Count < 1)
                return;

            int availableIndex = current;
            current += direction;
            if (current < 0)
                current = availableItems.Count - 1;
            current %= availableItems.Count;

            UpdateSelected(current);
        }

        public void OnSelectInput()
        {
            if (selectedTrack == null)
                return;

            TapeBoxItem item = availableItems[current % availableItems.Count];
            selectedTrack.EnqueueTape(item.BeatTape, this);

            if (!item.BeatTape.silent)
                availableItems.Remove(item);
            UpdateAvailables();
            current = 0;

            Show (null, false);
        }

        public void RestoreToAvailables (BeatTapeScriptableObject beatTapeData)
        {
            TapeBoxItem item = items.Find( (i) => i.BeatTape == beatTapeData ); 
            if (item == null)
                return;

            bool alreadyHas = availableItems.Contains( item );
            if (alreadyHas)
                return;

            availableItems.Add(item);
            UpdateAvailables();
        }
    }
}
