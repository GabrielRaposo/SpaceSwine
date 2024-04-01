using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricLock : Lock
{
    [Header("Electric Lock")]
    [SerializeField] ElectricMind mind;
    [SerializeField] Transform connectionTransform;

    [Header("Animator")]
    [SerializeField] Animator baseAnimator;
    [SerializeField] Animator lockHoleAnimator;
    [SerializeField] Animator sphereAnimator;

    bool active;

    protected override void Start()
    {
        base.Start();    

        if (!baseAnimator || !lockHoleAnimator || !sphereAnimator)
        {
            gameObject.SetActive(false);
            return;
        }

        SetState (active = true);
    }

    public void Setup (ElectricMind mind)
    {
        this.mind = mind;
        SetState (active = true);
    }

    public void SetState (bool active)
    {
        if (active)
        {
            baseAnimator.SetBool("Open", false);
            baseAnimator.SetTrigger("Reset");
        }
        else
        {
            baseAnimator.SetBool("Reset", false);
            baseAnimator.SetTrigger("Open");
        }

        lockHoleAnimator.SetBool("On", active);
        sphereAnimator.SetBool("On", active);

        this.active = active;
    }

    public override bool Collect (Collectable collectable)
    {
        base.Collect (collectable);

        SetState (false);
        
        //if (mind)
        //    mind.SetActivation(false);    
        

        return true;
    }

    public void TurnOff()
    {
        //SetState (false);

        if (mind)
            mind.SetActivation(false, silent: false);
    }

    public Transform GetConnectionPoint()
    {
        if (connectionTransform != null)
            return connectionTransform;

        return transform;
    }
}
