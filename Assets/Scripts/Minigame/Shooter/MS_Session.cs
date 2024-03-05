﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace Shooter
{
    public class MS_Session : MonoBehaviour
    {
        [SerializeField] bool specialSession;
        [SerializeField] int scoreReward;

        int completion;
        int totalCompletion;

        MS_Enemy[] enemies;
        MS_Sun sunEnemy;
        MS_SessionManager sessionManager;

        public UnityAction OnPreReset;
        public UnityAction OnReset;
        public UnityAction OnVanish;
        public UnityAction OnAfterVanish;

        public void Setup (MS_SessionManager sessionManager)
        {
            this.sessionManager = sessionManager;

            if (!specialSession)
            {
                if (enemies == null || enemies.Length < 1)
                    enemies = GetComponentsInChildren<MS_Enemy>();

                if (enemies.Length < 1)
                    return;

                foreach (MS_Enemy e in enemies)
                    e.Setup(this);
            }
            else
            {
                if (sunEnemy == null)
                    sunEnemy = GetComponentInChildren<MS_Sun>();

                if (!sunEnemy)
                    return;

                sunEnemy.Setup(this);
            }

            gameObject.SetActive(true);

            if (OnPreReset != null)
                OnPreReset.Invoke();

            transform.localPosition = Vector3.up * 10f;

            Sequence s = DOTween.Sequence();
            s.AppendInterval(.3f);
            s.Append( transform.DOLocalMoveY(endValue: 0f, duration: .35f).SetEase(Ease.OutCirc) );
            s.OnComplete( () => 
            {
                MS_SessionManager.OnSessionTransition = false;
                totalCompletion = completion = (specialSession ? 1 : enemies.Length);

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

            MS_ScoreManager.Instance.ChangeScore (scoreReward);
            MS_SessionManager.OnSessionTransition = true;
            MS_StageTimer.AddTime ( specialSession ? 6 : totalCompletion * 2);
            MS_StageTimer.SetSessionBlink();

            this.WaitSeconds (duration: .35f, action: () => 
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
