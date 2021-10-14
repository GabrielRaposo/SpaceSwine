using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jumper
{
    public class MJ_Hazard : MonoBehaviour
    {
        [System.Serializable]
        public struct HazardType
        {
            public Sprite sprite;
            public float radius;
        }

        [SerializeField] List<HazardType> hazardTypes;

        float rotationSpeed;

        void Start()
        {
            if (hazardTypes.Count < 1)
                return;

            int index = Random.Range(0, hazardTypes.Count);
        
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                spriteRenderer.sprite = hazardTypes[index].sprite;
                spriteRenderer.flipX = Random.Range(0, 2) == 0;
                spriteRenderer.flipY = Random.Range(0, 2) == 0;
            }

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            if (collider)
                collider.radius = hazardTypes[index].radius;

            rotationSpeed = Random.Range(25, 55) * (Random.Range(0,2) == 0 ? 1 : -1);
        }

        void FixedUpdate()
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.fixedDeltaTime);
        }
    }
}