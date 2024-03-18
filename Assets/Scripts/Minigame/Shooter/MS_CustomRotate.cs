using Shooter;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

public class MS_CustomRotate : CustomRotate
{
    [SerializeField] bool randomizeStartingRotation;

    protected override void Start()
    {
            moveOnStart = true;

            MS_Session session = GetComponentInParent<MS_Session>();
            if (session)
            {
                session.OnReset +=  () => 
                {
                    gameObject.SetActive(true);
                    Restart();
                    enabled = true;
                };

                session.OnVanish += () => 
                {
                    enabled = false;
                };

                session.OnAfterVanish += () =>
                {
                    if (randomizeStartingRotation)
                        startingRotation = Random.Range(0, 360);

                    gameObject.SetActive(true);
                    Restart();
                    FixedUpdate();
                    enabled = false;
                };

                if (MS_SessionManager.OnSessionTransition)
                    return;

                Restart();
            }

            enabled = false;
    }
}
