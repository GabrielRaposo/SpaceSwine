using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllignParticleWithNPC : MonoBehaviour
{
    [SerializeField] bool allignOnUpdate;
    [SerializeField] float deltaAngle;
    [SerializeField] ParticleSystem ps;

    void Start()
    {
        if (!ps)
        {
            enabled = false;
            return;
        }

        Allign();
    }

    private void Update() 
    {
        if (!allignOnUpdate)
            return;

        Allign();
    }

    private void Allign()
    {
        float angle = Vector2.SignedAngle (Vector2.up, transform.up) + deltaAngle;
        ParticleSystem.MainModule mainModule = ps.main;
        mainModule.startRotation = angle * Mathf.Deg2Rad * -1f;
    }
}
