using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTracker : MonoBehaviour
{
    [SerializeField] int max;
    [SerializeField] bool onlyOnChange;
    [SerializeField] Vector3 offset;
    [SerializeField] float minDistance;

    List<Vector2> positions;
    List<float> rotations;
    Vector2 previousPos = Vector2.zero;

    void Start()
    {
        positions = new List<Vector2>();
        rotations = new List<float>();
    }

    void FixedUpdate()
    {
        if (onlyOnChange) 
        {
            //if (GetAnchoredPosition() == (Vector3) previousPos)
            if (Vector3.Distance(GetAnchoredPosition(), previousPos) < minDistance)
                return;
            previousPos = GetAnchoredPosition();
        }

        positions.Add(GetAnchoredPosition());
        if (positions.Count > max)
            positions.RemoveAt(0);

        rotations.Add(transform.eulerAngles.z);
        if (rotations.Count > max)
            rotations.RemoveAt(0);
    }

    public (Vector2 Position, float Angle) GetDataAtPercent (float percent)
    {
        if (positions.Count < 1)
            return (GetAnchoredPosition(), transform.eulerAngles.z);

        int roundedPercent = Mathf.RoundToInt( percent * max );
        if (roundedPercent >= positions.Count)
            return (positions[positions.Count - 1], rotations[rotations.Count - 1]);

        return (positions[roundedPercent % positions.Count], rotations[roundedPercent % rotations.Count]);
    }

    private Vector3 GetAnchoredPosition() 
    { 
        if (offset == Vector3.zero)
            return transform.position;

        return transform.position + (Vector3) RaposUtil.AlignWithTransform(transform, offset);
    }

}
