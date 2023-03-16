using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatNavigationItem : MonoBehaviour
{
    const float MAX_THRESHOLD = 80f;

    [System.Serializable]
    public class NavigationData
    {
        public BeatNavigationItem targetItem;
        public Transform anchor;

        public Vector3 Direction (Vector3 origin)
        {
            if (!targetItem)
                return Vector2.zero;

            return (targetItem.transform.position - origin).normalized;
        }

        public Vector3 AnchoredDirection (Vector2 origin)
        {
            if (anchor == null)
                return Direction(origin);

            return Direction(anchor.position);
        }
    }

    [SerializeField] List <NavigationData> datas;
    [SerializeField] BeatTapeCursor cursor;

    public void Setup()
    {
        if (datas.Count < 1)
            return;

        foreach(NavigationData data in datas) 
            data.anchor = cursor.transform;

        cursor.InitArrows(datas);
    }

    private void OnDrawGizmosSelected () 
    {
        if (datas.Count < 1)
            return;

        foreach (NavigationData data in datas)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine (transform.position, transform.position + data.Direction(transform.position));
        }
    }

    public BeatNavigationItem FindItemOnDirection (Vector2 direction)
    {
        (BeatNavigationItem item, float difference) output = (null, Mathf.Infinity);
        foreach (NavigationData data in datas)
        {
            float angle = Vector2.SignedAngle( direction, data.AnchoredDirection(transform.position) );
            angle = Mathf.Abs(angle);

            if (angle > MAX_THRESHOLD )
                continue;

            if (angle < output.difference)
            {
                output.item = data.targetItem;
                output.difference = angle;
            }
        }

        return output.item;
    }
}
