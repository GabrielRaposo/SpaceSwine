using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace Shooter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MS_Bullet : MonoBehaviour
    {
        [SerializeField] int damage;
        [SerializeField] ParticleSystem trailPS;
        [SerializeField] MS_DettachableEffect dettachableEffect;

        bool insideArea;
        bool restoredAmmo;

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

            restoredAmmo = false;
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

            MS_TimeBonus timeBonus = collision.GetComponent<MS_TimeBonus>();
            if (timeBonus)
            {
                timeBonus.Collect();
            }

            MS_Enemy enemy = collision.GetComponent<MS_Enemy>();
            if (enemy)
            {
                enemy.TakeDamage(damage, restoredAmmo ? 1 : 0);

                if (/*!restoredAmmo &&*/ player)
                {
                    restoredAmmo = true;
                    player.RestoreAmmo();
                }

                MS_ComboManager.Instance.NotifyHit();
                return;
            }

            MS_Sun sun = collision.GetComponent<MS_Sun>();
            if (sun)
            {
                sun.TakeDamage(damage, restoredAmmo? 1 : 0);

                Vanish();
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

            if (dettachableEffect)
                dettachableEffect.Call();

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
            if (trailPS && trailPS.gameObject)
                trailPS.Stop();
        }
    }
}
