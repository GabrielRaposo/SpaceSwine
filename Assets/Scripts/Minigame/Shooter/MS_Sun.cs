using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public class MS_Sun : MonoBehaviour
    {
        [SerializeField] int scoreReward;
        [SerializeField] int startingHP = 1;

        int HP;

        MS_Session session;

        public void Setup (MS_Session session)
        {
            this.session = session;

            HP = startingHP;
            gameObject.SetActive(true);

            //SetBlinkingState(false);
        }

        void Update()
        {
        
        }

        public void TakeDamage (int value, int scoreBonus) 
        {
            HP -= value;

            MS_ScoreManager.Instance.ChangeScore (scoreReward + scoreBonus);

            if (HP > 0)
            {
                // On Take Damage
                //if (HitPS)
                //    HitPS.Play();

                //blinkingCount = blinkingDuration;
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

            if (session)
                session.NotifyProgress();
        }
    }
}
