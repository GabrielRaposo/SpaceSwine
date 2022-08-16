using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwiseEventPublicAccess : MonoBehaviour
{
    [SerializeField] List<AK.Wwise.Event> events; 

    public void PlayEvent (int id)
    {
        if (events.Count < 1)
            return;

        events[id % events.Count].Post(gameObject);
    }
}
