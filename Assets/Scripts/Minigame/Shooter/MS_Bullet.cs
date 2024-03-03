using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MS_Bullet : MonoBehaviour
    {
        Rigidbody2D rb;
        MS_Player player;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Shoot (MS_Player player, Vector2 position, Vector2 velocity)
        {
            // reset states

            this.player = player;

            transform.position = position;
            gameObject.SetActive(true);
            rb.velocity = velocity;
        }

        private void SelfDestruct()
        {
            // Call Destroy animation

            if (player)
                player.OnAmmoDestroyed();
        }
    }
}
