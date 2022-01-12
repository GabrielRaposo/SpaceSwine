using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LinearSequenceTool : MonoBehaviour
{
    public List<ObjectPoint> objects;
    public List<TurningPoint> turningPoints;

    public void OnValidate()
    {
        if (objects == null) objects = new List<ObjectPoint>();
        foreach (ObjectPoint objectPoint in objects)
        {
            if(objectPoint.obj)
                objectPoint.obj.transform.position = objectPoint.position;
        }
    }

    private void OnDrawGizmos()
    {
        

        for (int i = 0; i < turningPoints.Count-1; i++)
        {
            Gizmos.color = Color.green;
            var nextPointPos = turningPoints[i+1].position;
            
            Gizmos.DrawLine(turningPoints[i].position, nextPointPos);
            
            
            //var dir = nextPointPos - turningPoints[i].position;
            var dir = Vector3.up;
            var angle = Mathg.VectorToAngle(dir)+turningPoints[i].angle;
            
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(nextPointPos, nextPointPos+ Mathg.AngleToDirection(angle));
            
            Gizmos.color = new Color(0.45f,0.45f,1f);
            Gizmos.DrawLine(nextPointPos, nextPointPos+ Mathg.AngleToDirection(angle+180f));

        }
    }
}



[System.Serializable]
public class ObjectPoint
{
    public Vector3 position;
    public GameObject obj;
}

[System.Serializable]
public class TurningPoint
{
    public Vector3 position;
    [Range(0f,360f)]public float angle;
}