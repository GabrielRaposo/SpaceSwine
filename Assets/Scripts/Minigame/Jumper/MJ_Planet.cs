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

        static float DifficultyRatio = 1.5f;

        public static void ResetDifficulty()
        {
            DifficultyRatio = 1.5f;
        }

        public static void AddDifficulty()
        {
            DifficultyRatio += .125f;
        }

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
            transform.Rotate(Vector3.forward * rotationSpeed * DifficultyRatio * Time.fixedDeltaTime);
            //rotationAnchor.Rotate(Vector3.forward * rotationSpeed * Time.fixedDeltaTime);
        }

        private void OnDrawGizmos() 
        {
            if (rotationSpeed == 0)
                return;

            Vector2 upPosition = transform.position + Vector3.up * .75f;
            Vector2 offset = Vector2.left * 1f * (rotationSpeed > 0 ? 1 : -1);
            Gizmos.DrawLine( upPosition, upPosition + offset );
            Gizmos.DrawWireSphere (upPosition + offset, .1f );
        }
    }
}
