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
        [SerializeField] AK.Wwise.Event slideInAKEvent;
        [SerializeField] AK.Wwise.Event slideOutAKEvent;
        [SerializeField] AK.Wwise.Event navigationAKEvent;
        [SerializeField] Transform cursor;
        [SerializeField] GameObject boxTapePrefab;
        [SerializeField] DuctTapeLabel labelDisplay;
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
            // TO-DO: Adicionar interação com fitas desbloqueadas
            
            availableItems = new List<TapeBoxItem>();

            foreach (TapeBoxItem item in items)
                availableItems.Add(item);
        }

        private void UpdateAvailables (TapeBoxItem currentItem)
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
                item.SetHighlighted(false, dontOverride: true);
                item.gameObject.SetActive( availableItems.Contains(item) );
            }

            {
                if (currentItem == null)
                    return;

                int index = 0;
                if (availableItems.Contains(currentItem))
                {
                    index = availableItems.FindIndex( (item) => currentItem == item);
                }
                current = index;
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
                this.selectedTrack = selectedTrack;
                
                if (value)
                    slideInAKEvent?.Post(gameObject);
                else
                    slideOutAKEvent?.Post(gameObject);

                UpdateValidCurrent();
                UpdateSelected( value ? current : -1 );
            }

            sequence = DOTween.Sequence();
            sequence.Append( transform.DOLocalMoveY(value ? shownY : hiddenY, duration).SetEase(Ease.OutBounce) );

            sequence.OnComplete( () => 
            {
                SetShownPosition(value);
            });
        }

        private void UpdateValidCurrent()
        {
            current %= availableItems.Count;
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
                {
                    labelDisplay.SetText(item.BeatTape.title);
                    labelDisplay.Show();
                }

                if (tapePreviewDisplay)
                    tapePreviewDisplay.sprite = item.BeatTape.frontalSprite;
            }
            else
            {
                if (labelDisplay)
                    labelDisplay.Hide();

                if (tapePreviewDisplay)
                    tapePreviewDisplay.sprite = null;
            }
        }

        public void MoveCursor (int direction)
        {
            if (availableItems.Count < 1)
                return;

            current += direction;
            if (current < 0)
                current = availableItems.Count - 1;
            current %= availableItems.Count;

            if (navigationAKEvent != null)
                navigationAKEvent.Post(gameObject);

            UpdateSelected(current);
        }

        public void OnSelectInput()
        {
            if (selectedTrack == null)
                return;

            TapeBoxItem item = availableItems[current % availableItems.Count];
            selectedTrack.EnqueueTape(item.BeatTape, this);

            UpdateValidCurrent();

            if (!item.BeatTape.silent)
            {
                item.SetHighlighted(false);
                availableItems.Remove(item);
            }

            UpdateAvailables(null);

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

            UpdateValidCurrent();
            TapeBoxItem currentItem = availableItems[current];

            availableItems.Add(item);

            UpdateAvailables(currentItem);
        }
    }
}
