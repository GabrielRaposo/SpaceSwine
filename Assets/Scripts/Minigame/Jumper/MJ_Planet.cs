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
            current.position = rotationAnchor.position + Vector3.up * .95f;

            this.current = current;
        }

        private void FixedUpdate() 
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.fixedDeltaTime);
            //rotationAnchor.Rotate(Vector3.forward * rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
