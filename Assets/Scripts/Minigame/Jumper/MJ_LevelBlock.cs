using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jumper
{
    public class MJ_LevelBlock : MonoBehaviour
    {
        public float lowerBorder = 0;
        public float upperBorder = 1;

        [HideInInspector] public MJ_Player player;
        [HideInInspector] public MJ_BlockGroup originalParent;

        private void Update() 
        {
            if (!player)
                return;

            if (FitPosition + 5 < player.transform.position.y)
            {
                gameObject.SetActive(false);
                
                if (originalParent != null)
                    transform.SetParent(originalParent.transform);
            }
        }

        public float FitPosition
        {
            get
            {
                return transform.position.y + upperBorder;
            }
        }

        private void OnDrawGizmos() 
        {
            float offset = 5f;

            Gizmos.color = Color.green;
            Vector3 lowerPosition = transform.position + Vector3.up * lowerBorder;
            Gizmos.DrawLine(lowerPosition + Vector3.left * offset, lowerPosition + Vector3.right * offset);
            Gizmos.DrawWireCube(lowerPosition, .5f * Vector3.one);
            lowerPosition += Vector3.up;
            Gizmos.DrawLine(lowerPosition + Vector3.left * offset, lowerPosition + Vector3.right * offset);

            Gizmos.color = Color.yellow;
            Vector3 upperPosition = transform.position + Vector3.up * upperBorder;
            Gizmos.DrawLine(upperPosition + Vector3.left * offset, upperPosition + Vector3.right * offset);
            Gizmos.DrawWireCube(upperPosition, .25f * Vector3.one);
            upperPosition += Vector3.down;
            Gizmos.DrawLine(upperPosition + Vector3.left * offset, upperPosition + Vector3.right * offset);
        }
    }

}
