using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRotateBursts : MonoBehaviour
{
    [SerializeField] float startDelay;
    [SerializeField] float anglePerSpin;
    [SerializeField] int spinFrames;
    [SerializeField] float spinCooldown;
    [SerializeField] bool moveOnStart;

    float startingRotation;
    float startingT = 0; 
    float t;
    float startupCount;
    
    int frameCount;

    bool running;
    bool initiated;

    private void OnEnable() 
    {
        if (anglePerSpin == 0)
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

    private void Start() 
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
        running = moveOnStart;
        frameCount = 0;
    }

    private void FixedUpdate() 
    {
        if (!running)
            return;

        if (startupCount > 0)
        {
            startupCount -= Time.fixedDeltaTime;
            return;
        }
        
        t += Time.fixedDeltaTime;
        if (t < spinCooldown)
            return;

        if (frameCount < spinFrames)
        {
            transform.Rotate ( Vector3.forward * (anglePerSpin / spinFrames) );
            frameCount++;
            return;
        }

        // --
        frameCount = 0;
        t = 0;
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
