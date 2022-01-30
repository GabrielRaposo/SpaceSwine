using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryParticle : MonoBehaviour
{
    public float timeLimit;
    
    private void Start()
    {
        RaposUtil.WaitSeconds(timeLimit, DestroySelf);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
    
}
