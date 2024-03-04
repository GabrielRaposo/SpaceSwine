using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace Shooter
{
    public class MS_Session : MonoBehaviour
    {
        [SerializeField] int scoreReward;

        int completion;

        MS_Enemy[] enemies;
        MS_SessionManager sessionManager;

        public UnityAction OnReset;
        public UnityAction OnVanish;
        public UnityAction OnAfterVanish;

        public void Setup (MS_SessionManager sessionManager)
        {
            this.sessionManager = sessionManager;

            if (enemies == null || enemies.Length < 1)
                enemies = GetComponentsInChildren<MS_Enemy>();

            if (enemies.Length < 1)
                return;

            foreach (MS_Enemy e in enemies)
                e.Setup(this);

            gameObject.SetActive(true);

            transform.localPosition = Vector3.up * 10f;

            Sequence s = DOTween.Sequence();
            s.Append( transform.DOLocalMoveY(endValue: 0f, duration: .5f).SetEase(Ease.OutCirc) );
            s.OnComplete( () => 
            {
                MS_SessionManager.OnSessionTransition = false;
                completion = enemies.Length;

                if (OnReset != null) 
                    OnReset.Invoke();
            });
        }

        public void NotifyProgress()
        {
            completion--;

            if (completion > 0)
                return;

            if (OnVanish != null)
                OnVanish.Invoke();

            MS_ScoreManager.Instance.ChangeScore(scoreReward);
            MS_SessionManager.OnSessionTransition = true;

            this.WaitSeconds (duration: .5f, action: () => 
            {
                if (sessionManager != null)
                    sessionManager.NotifyCompletedSession();

                gameObject.SetActive (false);
                
                if (OnAfterVanish != null)
                    OnAfterVanish.Invoke();
            });
        }
    }
}
