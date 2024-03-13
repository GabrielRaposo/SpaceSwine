using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.WebRequestMethods;

namespace Shooter
{
    public class MS_Enemy : MonoBehaviour
    {   
        const string BLINKING_TAG = "Blinking";

        //[SerializeField] int scoreReward;
        [SerializeField] int startingHP = 1;
        [SerializeField] float blinkingDuration;

        [Header("References")]
        [SerializeField] MS_DettachableEffect vanishEffect;
        [SerializeField] Animator mainAnimator;
        [SerializeField] Animator floatAnimator;
        [SerializeField] Collider2D coll2D;
        [SerializeField] ParticleSystem HitPS;

        [Header ("Audio")]
        [SerializeField] AK.Wwise.Event OnDamageAKEvent;
        [SerializeField] AK.Wwise.Event OnVanishAKEvent;

        int HP;
        MS_Session session;

        float blinkingCount;


        public void Setup (MS_Session session, int level)
        {
            this.session = session;

            HP = SetHealthByLevel(level);
            gameObject.SetActive(true);

            SetBlinkingState(false);
        }

        private int SetHealthByLevel(int level)
        {
            if (level < 2)
                return 1;

            if (level < 5)
                return 2;
            return 3;
        }

        private void SetBlinkingState (bool value)
        {
            //if (coll2D)
            //    coll2D.enabled = !value;

            if (mainAnimator)
                mainAnimator.SetBool(BLINKING_TAG, value);

            if (floatAnimator)
                floatAnimator.enabled = !value;
        }

        private void Update()
        {
            transform.eulerAngles = Vector3.zero;

            if (blinkingCount > 0)
            {
                blinkingCount -= Time.deltaTime;

                if (blinkingCount <= 0)
                    SetBlinkingState(false);
            }
        }

        public void TakeDamage (int value, int scoreBonus) 
        {
            HP -= value;

            MS_ScoreManager.Instance.ChangeScore (1 + scoreBonus);

            if (HP > 0)
            {
                if (OnDamageAKEvent != null)
                    OnDamageAKEvent.Post(gameObject);

                if (HitPS)
                    HitPS.Play();

                blinkingCount = blinkingDuration;
                SetBlinkingState(true);

                return;
            }
        
            SelfDestruct();
        }

        private void Vanish()
        {
            gameObject.SetActive(false);
            
            if (vanishEffect)
                vanishEffect.Call();
        }

        public void SelfDestruct()
        {
            Vanish();

            if (OnVanishAKEvent != null)
                OnVanishAKEvent.Post(gameObject);

            if (session)
                session.NotifyProgress();
        }
    }
}
