using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationSpaceStreetShip : MonoBehaviour
{
    Vector3 start;
    Vector3 end;
    float duration;

    float count;

    public void Setup (Vector3 start, Vector3 end, float duration, Sprite sprite, float startCount)
    {
        this.start = start;
        this.end = end;

        this.duration = duration;
        count = startCount;

        GetComponent<SpriteRenderer>().sprite = sprite;
        transform.eulerAngles = Vector3.forward * (Vector2.SignedAngle (Vector2.up, end - start) );
    }

    void Update()
    {
        if (duration <= 0)
            return;

        transform.position = Vector3.Lerp( start, end, count/duration );

        count += Time.deltaTime;
        if (count > duration )
            count = 0;
    }
}
