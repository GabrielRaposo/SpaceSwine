using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jumper 
{
    public class MJ_Repeater : MonoBehaviour
    {
        [SerializeField] Vector2 spawnPoint;
        [SerializeField] Vector2 vanishPoint;
        [Space(5)]
        [SerializeField] float cycleDuration;

        void OnEnable()
        {
            List<Transform> repeatedItems = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                repeatedItems.Add(transform.GetChild(i));
            }

            if (repeatedItems.Count < 1)
                return;

            Vector2 direction = (vanishPoint - spawnPoint).normalized;
            float step = Vector2.Distance( vanishPoint, spawnPoint ) / repeatedItems.Count;
            for (int i = 0; i < repeatedItems.Count; i++)
            {
                repeatedItems[i].transform.position = (Vector2) transform.position + spawnPoint + (i * step * direction);
                MJ_Repeatable repeatable = repeatedItems[i].GetComponent<MJ_Repeatable>();
                if (repeatable == null)
                    repeatable = repeatedItems[i].gameObject.AddComponent<MJ_Repeatable>();
                
                repeatable.Setup
                (
                    (Vector2)transform.position + spawnPoint, 
                    (Vector2)transform.position + vanishPoint,
                    cycleDuration,
                    ((float)i / repeatedItems.Count) 
                );
            }

            // To-Do: posiciona hazards invisíveis offscreen
        }

        private void OnDrawGizmos() 
        {
            Gizmos.color = Color.yellow;
            
            Vector2 a = spawnPoint + (Vector2)transform.position;
            Vector2 b = vanishPoint + (Vector2)transform.position;
            
            Gizmos.DrawWireSphere (a,  .25f);
            Gizmos.DrawWireSphere (b, .25f);
            Gizmos.DrawLine (a, b);
        }
    }
}