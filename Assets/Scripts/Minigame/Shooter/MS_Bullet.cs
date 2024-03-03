using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MS_Bullet : MonoBehaviour
    {
        [SerializeField] int damage;
        [SerializeField] ParticleSystem trailPS;

        bool insideArea;

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

            trailPS.transform.SetParent (null);
            trailPS.transform.position = transform.position;
            trailPS.Play();
        }

        private void Update()
        {
            trailPS.transform.position = transform.position;
        }

        private void OnTriggerEnter2D (Collider2D collision)
        {
            if (collision.CompareTag("GameplayArea"))
            {
                insideArea = true;
                return;
            }

            if (collision.CompareTag("Hazard"))
            {
                SelfDestruct();
                return;
            }

            MS_Enemy enemy = collision.GetComponent<MS_Enemy>();
            if (enemy)
            {
                enemy.TakeDamage(damage);

                if (player)
                    player.RestoreAmmo();

                MS_ComboManager.Instance.NotifyHit();
                return;
            }

            MS_Bouncer bouncer = collision.GetComponent<MS_Bouncer>();
            if (bouncer)
            {
                rb.velocity = bouncer.ReflectVelocity (rb.velocity);
            }
        }

        public void Vanish()
        {
            trailPS.Stop();

            // -- Call Destroy animation

            insideArea = false;
            gameObject.SetActive(false);
        }

        private void SelfDestruct()
        {
            Vanish();

            if (player)
                player.OnAmmoDestroyed();
        }

        private void OnTriggerExit2D (Collider2D collision)
        {
            if (insideArea && collision.CompareTag("GameplayArea"))
            {
                SelfDestruct();
            }
        }

        private void OnDisable()
        {
            trailPS.Stop();
        }
    }
}
