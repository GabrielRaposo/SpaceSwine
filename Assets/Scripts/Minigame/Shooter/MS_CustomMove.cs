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

            Restart();

            MS_Session session = GetComponentInParent<MS_Session>();
            if (session)
            {
                session.OnReset +=  () => 
                {
                    Restart();
                };
            }

            OnStart?.Invoke();
        }
    }
}
