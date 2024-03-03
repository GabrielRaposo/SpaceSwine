using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Shooter
{
    public class MS_Session : MonoBehaviour
    {
        [SerializeField] int scoreReward;

        int completion;

        MS_Enemy[] enemies;
        MS_SessionManager sessionManager;

        public UnityAction OnReset;

        public void Setup (MS_SessionManager sessionManager)
        {
            this.sessionManager = sessionManager;

            if (enemies == null || enemies.Length < 1)
                enemies = GetComponentsInChildren<MS_Enemy>();

            if (enemies.Length < 1)
                return;

            foreach (MS_Enemy e in enemies)
                e.Setup(this);

            completion = enemies.Length;

            if (OnReset != null) 
                OnReset.Invoke();
        }

        public void NotifyProgress()
        {
            completion--;

            if (completion > 0)
                return;

            MS_ScoreManager.Instance.ChangeScore(scoreReward);

            if (sessionManager != null)
                sessionManager.NotifyCompletedSession();
        }
    }
}
