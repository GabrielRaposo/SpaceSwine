using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AnchorToPlanetWithRaycast : MonoBehaviour
{
    [SerializeField] float angle;
    [SerializeField] float offsetHeight;

    GravitationalPlanet currentPlanet;
    bool attached;

    void Start()
    {
        Setup();    
    }

    #if UNITY_EDITOR
    private void Update() 
    {
        if (Application.isPlaying)
            return;

        Attach();
    }
    #endif

    private void Setup()
    {
        if (!enabled)
            return;

        if (currentPlanet != null)
            return;

        (bool found, GravitationalPlanet planet) raycastCheck = CheckForPlanet();
        if (!raycastCheck.found)
            return;

        Attach();
    }

    private (bool, GravitationalPlanet) CheckForPlanet()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, .1f, 1 << LayerMask.NameToLayer("Gravity"));
        if (colliders.Length < 1)
            return (false, null);

        foreach (Collider2D coll in colliders)
        {
            GravityArea gravityArea = coll.GetComponent<GravityArea>();
            if (gravityArea == null)
                continue;
                
            currentPlanet = gravityArea.GetComponentInParent<GravitationalPlanet>();
            if (currentPlanet == null)
                continue;

            break;
        }

        if (currentPlanet == null)
            return (false, null);

        return (true, currentPlanet);
    }

    private void Attach()
    {
        if (!currentPlanet) 
        {
            attached = false;
            return;
        }

        (float planet, float gravity) radius = currentPlanet.GetAttributes();

        Vector2 direction = Vector2.zero;
        float localAngle = 0;

        if (!attached)
        {
            direction = (transform.position - currentPlanet.transform.position).normalized;
            localAngle = Vector2.SignedAngle(Vector2.up, direction);
            angle = localAngle;
        }
        else
        {
            direction = RaposUtil.RotateVector(Vector2.up, angle);
            localAngle = angle;
        }
        
        //Debug.Log("Set pos");
        transform.position = (Vector3)(direction * (radius.planet + offsetHeight)) + currentPlanet.transform.position;
        transform.localEulerAngles = Vector3.forward * localAngle;

        attached = true;
    }

    private void OnDisable() 
    {
        currentPlanet = null;
        attached = false;
    }
}
