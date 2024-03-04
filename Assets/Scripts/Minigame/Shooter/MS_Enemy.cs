using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public class MS_Enemy : MonoBehaviour
    {   
        [SerializeField] int scoreReward;
        [SerializeField] int startingHP = 1;
        [SerializeField] MS_DettachableEffect vanishEffect;

        int HP;
        MS_Session session;

        void Start()
        {
        
        }

        public void Setup (MS_Session session)
        {
            this.session = session;

            HP = startingHP;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            transform.eulerAngles = Vector3.zero;
        }

        public void TakeDamage (int value, int scoreBonus) 
        {
            HP -= value;

            MS_ScoreManager.Instance.ChangeScore (scoreReward + scoreBonus);

            if (HP > 0)
            {
                // On Take Damage
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

            if (session)
                session.NotifyProgress();
        }
    }
}
