using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRuler : MonoBehaviour
{
    public List<Transform> objects;
    public float radius;
    public float offsetRotation;
    public float sector;
    public bool rotateRadial;

    private void OnValidate()
    {
        if (objects == null) objects = new List<Transform>();
        if(objects.Count==0) return;

        if (radius <= 0) radius = 0f;

        var part = (360f-sector) / objects.Count;
        
        for (int i = 0; i < objects.Count; i++)
        {
            float angle = offsetRotation + part * i;
            var vec = Mathg.AngleToDirection(angle);
            
            if (!objects[i]) continue;
            objects[i].transform.position = transform.position + vec * radius;

            if (rotateRadial)
                objects[i].transform.eulerAngles = new Vector3(0f, 0f, angle-90f);

        }
    }

    public void GetAllChildren()
    {
        objects = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
            objects.Add(transform.GetChild(i));
        OnValidate();
    }

    public void CopyLastChild()
    {
        
    }

    void Update()
    {
        
    }
}
