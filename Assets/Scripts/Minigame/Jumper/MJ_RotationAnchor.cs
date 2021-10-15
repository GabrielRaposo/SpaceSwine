using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jumper
{
    public class MJ_RotationAnchor : MonoBehaviour
    {
        const float DIFF_MODIFIER = 1.0f;

        [SerializeField] float rotationSpeed;    

        void FixedUpdate()
        {
            transform.Rotate(Vector3.forward * rotationSpeed * DIFF_MODIFIER * Time.fixedDeltaTime);
        }

        private void OnDrawGizmos() 
        {
            Gizmos.DrawWireSphere (transform.position, .5f );

            if (rotationSpeed == 0)
                return;

            Vector2 upPosition = transform.position + Vector3.up * .5f;
            Vector2 offset = Vector2.left * 1f * (rotationSpeed > 0 ? 1 : -1);
            Gizmos.DrawLine( upPosition, upPosition + offset );
            Gizmos.DrawWireSphere (upPosition + offset, .1f );
        }
    }
}
