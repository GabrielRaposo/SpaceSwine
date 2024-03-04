using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomRotate : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float startDelay;
    [SerializeField] CustomEase customEase;
    [SerializeField] protected bool moveOnStart;

    float startingRotation;
    float startingT = 0; 
    float t;
    float startupCount;

    bool rotating;
    bool initiated;

    private void OnEnable() 
    {
        if (speed == 0)
        {
            enabled = false;
            return;
        }

        if (!initiated)
        {
            startingRotation = transform.eulerAngles.z;
            initiated = true;
        }
    }

    protected virtual void Start() 
    {
        Restart();

        Round round = GetComponentInParent<Round>();
        if (round)
            round.OnReset += Restart;
    }

    public void Restart()
    {
        if (initiated)
            transform.eulerAngles = Vector3.forward * startingRotation;

        startupCount = startDelay;
        t = startingT; 
        rotating = moveOnStart;
    }

    public void StartRotation()
    {
        if (!moveOnStart)
        {
            t = startingT;    
        }
        startupCount = startDelay;
        rotating = true;
    }

    public void ResumeRotation()
    {
        rotating = true;
    }

    public void PauseRotation()
    {
        rotating = false;
    }

    public void StopRotation()
    {
        t = startingT;    
        rotating = false;
    }

    protected void FixedUpdate() 
    {
        if (!rotating)
            return;
        
        if (startupCount > 0)
        {
            startupCount -= Time.fixedDeltaTime;
            return;
        }

        transform.Rotate( Vector3.forward * speed * Time.deltaTime );
    }

    public void SetStartingT (float startingT)
    {
        this.startingT = startingT;
    } 

    private void OnDisable() 
    {
        StopAllCoroutines();

        if (initiated)
        {
            transform.eulerAngles = Vector3.forward * startingRotation;
        }    
    }
}
