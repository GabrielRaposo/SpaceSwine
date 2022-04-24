﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesQueue : MonoBehaviour
{
    const int MAX_QUANT = 4;

    [SerializeField] GameObject itemPrefab; 
    [SerializeField] TransformTracker tracker;

    List<CollectableQueueItem> queue;
    List<GameObject> objectList; 

    private void Start() 
    {
        objectList = new List<GameObject>();

        for (int i = 0; i < MAX_QUANT; i++)
        {
            GameObject _obj = Instantiate(itemPrefab, transform);
            _obj.SetActive(false);
            objectList.Add(_obj);

            //temp
            _obj.transform.localPosition = Vector2.left * ((i + 1) / 2f); 
        }

        itemPrefab.SetActive(false);
    }

    public bool AddToQueue (Collectable collectable)
    {
        if (queue == null)
            queue = new List<CollectableQueueItem>();

        if (queue.Count >= MAX_QUANT)
            return false;

        GameObject currentObject = objectList[queue.Count];
        CollectableQueueItem queueItem = currentObject.GetComponent<CollectableQueueItem>();
        if (!queueItem)
            return false;

        queue.Add(queueItem);

        queueItem.Initiate(collectable);

        //float fractionStep = queue.Count / 2f;
        //if (queue.Count > 0)
        //{
        //    fractionStep = 1f / (MAX_QUANT + 1);
        //}

        //float trackPercent = (queue.Count + 1) * fractionStep;
        //Debug.Log("trackPercent: " + trackPercent);
        //queueItem.SetTracker(tracker, 1f - trackPercent);

        UpdateTrackPercents();

        return true;
    }

    private void UpdateTrackPercents()
    {
        if (queue.Count < 1)
            return;

        for (int i = 0; i < queue.Count; i++)
        {
            float fractionStep = fractionStep = 1f / (MAX_QUANT + 1);
            float trackPercent = (i + 1) * fractionStep;

            //Debug.Log("trackPercent: " + trackPercent);
            queue[i].SetTracker(tracker, 1f - trackPercent);
        }
    }

    public Collectable GetFromQueue()
    {
        if (queue.Count < 1)
            return null;

        CollectableQueueItem queueItem = queue[0];
        queue.RemoveAt(0);
        
        Collectable collectable = queueItem.Use();

        // Manda o gameObject pro final da lista
        objectList.Remove(queueItem.gameObject);
        objectList.Add(queueItem.gameObject);

        UpdateTrackPercents();

        return collectable;

        // SetInteractable(true);
    }

    public void ResetStates()
    {
        queue = new List<CollectableQueueItem>();
    }
}
