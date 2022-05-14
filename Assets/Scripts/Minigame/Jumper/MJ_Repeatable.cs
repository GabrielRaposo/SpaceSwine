using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jumper 
{
    public class MJ_Repeatable : MonoBehaviour
    {
        Vector2 spawnPoint; 
        Vector2 vanishPoint; 
        float cycleDuration;
        float percent;

        public void Setup (Vector2 spawnPoint, Vector2 vanishPoint, float cycleDuration, float startingPercent)
        {
            this.spawnPoint = spawnPoint;
            this.vanishPoint = vanishPoint;
            this.cycleDuration = cycleDuration;

            percent = startingPercent;
            //Debug.Log("startingPercent: " + startingPercent);
        }    

        void FixedUpdate()
        {
            percent += Time.fixedDeltaTime / cycleDuration;
            if (percent > 1)
                percent = 1;
            
            transform.position = Vector2.Lerp(spawnPoint, vanishPoint, percent);

            if (percent == 1)
            {
                percent = 0;
            }
        }
    }
}
