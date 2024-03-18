using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Shooter
{

    public class MS_CustomMove : CustomMove
    {
        protected override void Start() 
        {
            moveOnStart = true;
            customEase = CustomEase.EaseInOut;

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
}
