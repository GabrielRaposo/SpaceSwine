using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter
{
    public class MS_Enemy : MonoBehaviour
    {   
        [SerializeField] int scoreReward;
        [SerializeField] int startingHP = 1;

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

        public void TakeDamage (int value) 
        {
            HP -= value;

            MS_ScoreManager.Instance.ChangeScore (scoreReward);

            if (HP > 0)
            {
                // On Take Damage
                return;
            }
        
            SelfDestruct();
        }

        public void SelfDestruct()
        {
            gameObject.SetActive(false);

            if (session)
                session.NotifyProgress();
        }
    }
}
