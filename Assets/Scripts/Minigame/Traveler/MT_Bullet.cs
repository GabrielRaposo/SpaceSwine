using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Traveler
{
    public class MT_Bullet : MonoBehaviour
    {
        public bool FriendlyFire { get; private set; }
        
        Rigidbody2D rb;

        public void Shoot (Vector2 velocity, Vector2 origin)
        {
            FriendlyFire = false;
            transform.position = origin;

            gameObject.SetActive (true);

            rb = GetComponent<Rigidbody2D>();
            rb.velocity = velocity;

            this.WaitSeconds (1f, () => FriendlyFire = true);
        }
    }
}
