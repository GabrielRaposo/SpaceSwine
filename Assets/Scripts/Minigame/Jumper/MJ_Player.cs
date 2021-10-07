using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jumper
{
    public class MJ_Player : MonoBehaviour
    {
        bool attached;

        void Start()
        {
        
        }

        private void OnTriggerStay2D (Collider2D collision) 
        {
            if (!attached)
            {
                MJ_Planet planet = collision.GetComponent<MJ_Planet>();
                if (!planet)
                    return;

                planet.Attach (transform);
                attached = true;
            }
        }

    }
}
