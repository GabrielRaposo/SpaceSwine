using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTracker : MonoBehaviour
{
    [SerializeField] int max;
    [SerializeField] bool onlyOnChange;

    List<Vector2> positions;

    void Start()
    {
        positions = new List<Vector2>();
    }

    void FixedUpdate()
    {
        if (onlyOnChange && !transform.hasChanged)
            return;

        positions.Add(transform.position);
        if (positions.Count > max)
            positions.RemoveAt(0);
    }

    public Vector2 GetPositionAtPercent (float percent)
    {
        if (positions.Count < 1)
            return transform.position;

        int roundedPercent = Mathf.RoundToInt( percent * max );
        if (roundedPercent >= positions.Count)
            return positions[positions.Count - 1];

        return positions[roundedPercent % positions.Count];
    }
}
