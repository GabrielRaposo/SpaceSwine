using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jumper
{
    public class MJ_Planet : MonoBehaviour
    {
        [SerializeField] Transform rotationAnchor;
        [SerializeField] float rotationSpeed;

        Transform current;

        private void Start() 
        {
            if (!rotationAnchor)
            {
                enabled = false;
                return;
            }

            SpriteRenderer spriteRenderer = rotationAnchor.GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                spriteRenderer.flipX = rotationSpeed < 0;
            }
        }

        public void Attach (Transform current)
        {
            current.SetParent(rotationAnchor);
            
            Vector3 direction = (current.position - transform.position).normalized;
            current.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
            current.position = rotationAnchor.position + (direction * .95f);

            this.current = current;
        }

        public void Dettach (Transform current)
        {
            if (this.current != current)
                return;
            
            current.SetParent(null);
            current = null;
        }

        private void FixedUpdate() 
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.fixedDeltaTime);
            //rotationAnchor.Rotate(Vector3.forward * rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
