using System.Collections;
using System.Collections.Generic;
using Traveler;
using UnityEngine;

namespace Traveler
{
    public class MT_Barrier : MonoBehaviour
    {
        [SerializeField] float activeDuration;

        float t;
        Transform follow;

        public void Setup (Transform follow)
        {
            this.follow = follow;

            transform.SetParent(null);
            transform.position = follow.position;

            gameObject.SetActive(true);
            t = 0;
        }

        private void Update()
        {
            transform.position = follow.position;

            t += Time.deltaTime;

            if (t > activeDuration)
                gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D (Collider2D collision)
        {
            TriggerEvent (collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            TriggerEvent (collision);
        }

        private void TriggerEvent(Collider2D collision)
        { 
            MT_Bullet bullet = collision.GetComponent<MT_Bullet>();
            if (bullet && bullet.FriendlyFire)
            {
                bullet.Vanish();
                MT_ScoreManager.Instance.ChangeScore(1);
            }
        }
    }
}
