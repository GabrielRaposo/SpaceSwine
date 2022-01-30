using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpAndDown : MonoBehaviour
{
    [SerializeField] float distance;
    [SerializeField] float duration;

    int direction;
    float count;

    private Vector3 startPos;
    private bool keepTrackOfChildren;
    private List<Vector3> childrenPosition;

    private Round _round;
    
    void Start()
    {
        _round = GetComponentInParent<Round>();
        if (_round)
            _round.OnReset += OnReset;
        
        direction = 1; 
        count = 0;

        startPos = transform.localPosition;
        
        if (transform.childCount > 0)
        {
            keepTrackOfChildren = true;
            childrenPosition = new List<Vector3>();
            
            for (int i = 0; i < transform.childCount; i++)
            {
                childrenPosition.Add(transform.GetChild(i).transform.localPosition);
            }
        }
    }

    private void FixedUpdate() 
    {
        float step = (distance / duration) * direction * Time.fixedDeltaTime;
        transform.localPosition += Vector3.up * step;

        count += Time.fixedDeltaTime;
        if (count >= duration)
        {
            direction *= -1;
            count = 0;
        }
    }

    private void OnReset()
    {
        Debug.Log("Up dan down reset");
        
        direction = 1; 
        count = 0;
        
        transform.localPosition = startPos;

        if(!keepTrackOfChildren) return;
        
        for (int i = 0; i < childrenPosition.Count; i++)
        {
            transform.GetChild(i).localPosition = childrenPosition[i];
        }
        
    }
    
}
