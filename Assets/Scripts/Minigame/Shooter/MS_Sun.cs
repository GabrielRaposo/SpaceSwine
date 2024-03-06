using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public class MS_Sun : MonoBehaviour
    {
        [SerializeField] int scoreReward;
        [SerializeField] int startingHP = 1;
        [SerializeField] float blinkingDuration;
        
        [Header("References")]
        [SerializeField] GameObject visualComponent;
        [SerializeField] SpriteSwapper faceSwapper;
        [SerializeField] SpriteSwapper bodySwapper;
        [SerializeField] SpriteSwapper earsSwapper;

        [Header ("Audio")]
        [SerializeField] AK.Wwise.Event OnDamageAKEvent;
        [SerializeField] AK.Wwise.Event OnVanishAKEvent;

        int HP;

        MS_Session session;

        public void Setup (MS_Session session)
        {
            this.session = session;

            HP = startingHP;
            gameObject.SetActive(true);

            //SetBlinkingState(false);
        }

        public void TakeDamage (int value, int scoreBonus) 
        {
            HP -= value;

            MS_ScoreManager.Instance.ChangeScore (scoreReward + scoreBonus);

            if (HP > 0)
            {
                if (OnDamageAKEvent != null)
                    OnDamageAKEvent.Post(gameObject);

                StopAllCoroutines ();
                StartCoroutine (BlinkingRoutine());
                //SetBlinkingState(true);

                return;
            }
        
            SelfDestruct();
        }

        private void Vanish()
        {
            gameObject.SetActive(false);
            
            //if (vanishEffect)
            //    vanishEffect.Call();
        }

        public void SelfDestruct()
        {
            Vanish();

            if (OnVanishAKEvent != null)
                OnVanishAKEvent.Post(gameObject);

            if (session)
                session.NotifyProgress();
        }

        IEnumerator BlinkingRoutine ()
        {
            float step = .06f;
            float t = 0;

            if (faceSwapper) faceSwapper.SetSpriteState(1);

            while (t < blinkingDuration)
            {
                if (visualComponent) visualComponent.SetActive(true);
                if (bodySwapper) bodySwapper.SetSpriteState(1);
                if (earsSwapper) earsSwapper.SetSpriteState(1);

                yield return new WaitForSeconds (step);
                t += step;

                if (bodySwapper) bodySwapper.SetSpriteState(2);
                if (earsSwapper) earsSwapper.SetSpriteState(2);

                yield return new WaitForSeconds (step);
                t += step;

                if (visualComponent) visualComponent.SetActive(false);
                //if (bodySwapper) bodySwapper.SetSpriteState(0);
                //if (earsSwapper) earsSwapper.SetSpriteState(0);

                yield return new WaitForSeconds (step);
                t += step;
            }

            if (visualComponent) visualComponent.SetActive(true);
            if (faceSwapper) faceSwapper.SetSpriteState(0);
            if (bodySwapper) bodySwapper.SetSpriteState(0);
            if (earsSwapper) earsSwapper.SetSpriteState(0);
        }
    }
}
