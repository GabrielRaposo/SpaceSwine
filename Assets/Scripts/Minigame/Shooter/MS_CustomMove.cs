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
                    enabled = true;
                    Restart();
                };

                session.OnVanish += () => 
                {
                    Restart();
                    FixedUpdate();
                    enabled = false;
                    gameObject.SetActive(false);
                };

                if (MS_SessionManager.OnSessionTransition)
                    return;

                Restart();
            }
        }
    }
}
