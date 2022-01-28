using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class LinearRuler : MonoBehaviour
{
    public List<Transform> objects;
    [Range(0,360f)]public float rotation;
    public float lenght;

    private Vector3 startPos;
    private Vector3 finalPos;

    public bool allwaysDraw;

    private void OnValidate()
    {
        if (objects == null) objects = new List<Transform>();
        if(objects.Count <= 1) return;

        var directionVec = Mathg.AngleToDirection(rotation);

        var part = lenght / (objects.Count - 1);

        startPos = transform.position - directionVec * (part * (objects.Count - 1)/2f);
        
        for (int i = 0; i < objects.Count; i++)
        {
            if(objects[i] == null) continue;
            objects[i].transform.position = startPos + directionVec * part * i;
            finalPos = objects[i].transform.position;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(startPos, finalPos);
    }

    private void OnDrawGizmos()
    {
        if(allwaysDraw)
            OnDrawGizmosSelected();
    }

    private void Update()
    {
        OnValidate();
    }
}
