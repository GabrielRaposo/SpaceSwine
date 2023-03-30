using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricLine : MonoBehaviour
{   
    [SerializeField] ParticleSystem burst1PS;
    [SerializeField] ParticleSystem burst2PS;

    bool autoUpdate;

    Transform start; 
    Transform end; 

    SpriteRenderer[] srs;
    BoxCollider2D coll;

    private void Awake() 
    {
        srs = GetComponentsInChildren<SpriteRenderer>();
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
        //transform.position = start.transform.position;
        Vector3 distance = end.position - start.position;
        transform.position = start.transform.position + (distance / 2f);
        
        foreach(SpriteRenderer sr in srs)
            sr.size = new Vector2(distance.magnitude, sr.size.y);
        if (coll)
        {
            coll.offset = new Vector2(0, 0);
            coll.size = new Vector2(distance.magnitude, coll.size.y);
        }
        transform.eulerAngles = Vector3.forward * (Vector2.SignedAngle(Vector2.right, distance.normalized));
    }

    public void SetActivation (bool value)
    {
        //gameObject.SetActive (value);
        
        coll.enabled = value;

        foreach (SpriteRenderer sr in srs)
            sr.enabled = value;

        if (!value)
        {
            if (start == null || end == null)
                return;
            
            PlayBurst (burst1PS, end.position,   (end.position - start.position).normalized);
            PlayBurst (burst2PS, start.position, (start.position - end.position).normalized);
        }
    }

    private void PlayBurst (ParticleSystem particleSystem, Vector3 origin, Vector3 direction)
    {
        if (particleSystem == null)
            return;

        particleSystem.transform.position = origin;
        particleSystem.transform.eulerAngles = Vector3.forward * (Vector2.SignedAngle (Vector3.left, direction) - 15f);

        particleSystem.Play();
    }
}
