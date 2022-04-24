using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesQueue : MonoBehaviour
{
    const int MAX_QUANT = 7;

    [SerializeField] GameObject itemPrefab; 

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

        queueItem.Initiate(collectable);
        queue.Add(queueItem);

        return true;
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

        return collectable;

        // SetInteractable(true);
    }
}
