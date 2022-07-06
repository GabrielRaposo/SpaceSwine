using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricLine : MonoBehaviour
{   
    bool autoUpdate;

    Transform start; 
    Transform end; 

    SpriteRenderer sr;
    BoxCollider2D coll;

    private void Awake() 
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        coll = GetComponentInChildren<BoxCollider2D>();    
    }

    public void Setup (bool autoUpdate, Transform start, Transform end)
    {

        this.autoUpdate = autoUpdate;

        this.start = start;
        this.end   = end;

        UpdateLine();
    }

    private void Update() 
    {
        if (!autoUpdate)
            return;

        UpdateLine();
    }

    private void UpdateLine()
    {
        transform.position = start.transform.position;
        
        Vector2 direction = end.position - start.position;
        if (sr) 
            sr.size = new Vector2(direction.magnitude, sr.size.y);
        if (coll)
        {
            coll.offset = new Vector2(direction.magnitude/2, 0);
            coll.size = new Vector2(direction.magnitude, coll.size.y);
        }
        transform.eulerAngles = Vector3.forward * (Vector2.SignedAngle(Vector2.right, direction.normalized));
    }
}
