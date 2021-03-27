using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpAndDown : MonoBehaviour
{
    [SerializeField] float distance;
    [SerializeField] float duration;

    int direction;
    float count;

    void Start()
    {
        direction = 1; 
        count = 0;
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
}
