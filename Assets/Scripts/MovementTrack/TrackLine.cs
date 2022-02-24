using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackLine : MonoBehaviour
{
    public void Set (Vector2 pointA, Vector2 pointB)
    {
        transform.position = pointA;

        Vector2 direction = pointB - pointA;
        SpriteRenderer _renderer = GetComponent<SpriteRenderer>();
        _renderer.size = new Vector2(_renderer.size.x, direction.magnitude);

        float rotation = Vector2.SignedAngle(Vector2.up, direction);
        transform.eulerAngles = Vector3.forward * rotation;
    }
}
