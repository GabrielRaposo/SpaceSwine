using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AttachToPlanet : MonoBehaviour
{
    [SerializeField] float angle;
    
    bool attached;
    GravitationalPlanet planet;

    void Start()
    {
        Attach();
    }

    #if UNITY_EDITOR
    private void Update() 
    {
        if (Application.isPlaying)
            return;

        Attach();
    }
    #endif

    private void Attach()
    {
        planet = GetComponentInParent<GravitationalPlanet>();
        if (!planet) 
        {
            attached = false;
            return;
        }

        (float planet, float gravity) radius = planet.GetAttributes();

        Vector2 direction = Vector2.zero;
        float localAngle = 0;

        if (!attached)
        {
            direction = (transform.position - planet.transform.position).normalized;
            localAngle = Vector2.SignedAngle(Vector2.up, direction);
            angle = localAngle;
        }
        else
        {
            direction = RaposUtil.RotateVector(Vector2.up, angle);
            localAngle = angle;
        }
        
        transform.localPosition = direction * radius.planet;
        transform.localEulerAngles = Vector3.forward * localAngle;

        attached = true;
    }
}
