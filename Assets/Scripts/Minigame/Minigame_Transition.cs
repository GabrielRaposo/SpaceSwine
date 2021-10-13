using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace Minigame
{
    public class Minigame_Transition : MonoBehaviour
    {
        [SerializeField] float topY;
        [SerializeField] float midY;
        [SerializeField] float botY;
        [SerializeField] float duration;
        [Space(5)]
        [SerializeField] RectTransform mainAnchor;

        Sequence mainSequence;
    
        public static bool OnTransition;
        static Minigame_Transition Instance;

        private void Awake()
        {
            if (Instance)
            {
                //Destroy (gameObject);
                return;
            }

            Instance = this;
        }

        public static void Call(UnityAction action)
        {
            if (Instance)
            {
                Instance.LocalCall(action);
            }
            else
            {
                action?.Invoke();
            }
        }

        private void LocalCall(UnityAction action)
        {
            StartCoroutine( TransitionCoroutine(action) );
        }

        private IEnumerator TransitionCoroutine(UnityAction action)
        {
            OnTransition = true;
            bool done = false;

            if (!mainAnchor)
                yield break;

            if (mainSequence != null)
                mainSequence.Kill();

            mainAnchor.anchoredPosition = new Vector2 (mainAnchor.anchoredPosition.x, topY);

            // Mostra transição
            mainSequence = DOTween.Sequence();
            mainSequence.Append( mainAnchor.DOAnchorPosY(midY, duration).SetEase(Ease.Linear) );
            mainSequence.OnComplete( () => done = true );
            mainSequence.SetUpdate(isIndependentUpdate: true);

            yield return new WaitUntil( () => done );
            done = false;

            // Chamada de action 
            action?.Invoke();
            yield return new WaitForEndOfFrame();

            // Some com transição
            mainSequence = DOTween.Sequence();
            mainSequence.Append( mainAnchor.DOAnchorPosY(botY, duration).SetEase(Ease.Linear) );
            mainSequence.OnComplete( () => done = true );
            mainSequence.SetUpdate(isIndependentUpdate: true);

            yield return new WaitUntil( () => done );

            OnTransition = false;
        }
    }
}
