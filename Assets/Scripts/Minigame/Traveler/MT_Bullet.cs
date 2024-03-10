using Shooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Traveler
{
    public class MT_Bullet : MonoBehaviour
    {
        [SerializeField] bool rotateOnAim;
        [SerializeField] SpriteSwapper spriteSwapper;
        [SerializeField] MS_DettachableEffect vanishEffect;

        public bool FriendlyFire { get; private set; }
        
        Rigidbody2D rb;

        public void Shoot (Vector2 velocity, Vector2 origin)
        {
            spriteSwapper.SetSpriteState(0);

            FriendlyFire = false;
            transform.position = origin;

            gameObject.SetActive (true);

            rb = GetComponent<Rigidbody2D>();
            rb.velocity = velocity;

            if (rotateOnAim)
                transform.eulerAngles = Vector3.forward * Vector2.SignedAngle (Vector2.up, velocity);

            this.WaitSeconds (1f, ActivateFriendlyFire);
        }

        private void ActivateFriendlyFire()
        {
            spriteSwapper.SetSpriteState(1);
            FriendlyFire = true;
        }

        public void Vanish ()
        {
            if (vanishEffect)
                vanishEffect.Call();

            gameObject.SetActive(false);
        }

        public void Highlight()
        {
            spriteSwapper.SetSpriteState (2);
            spriteSwapper.ChangePrintOrder (4);
        }
    }
}
