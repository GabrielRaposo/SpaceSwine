using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Minigame
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GGSSplashScreen : MonoBehaviour
    {
        [SerializeField] float silenceDuration;
        [SerializeField] float logoDuration;
        [SerializeField] RawImage logo;

        CanvasGroup canvasGroup;

        void OnEnable()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            logo.enabled = false;
        }

        public void Call (UnityAction AfterSplash)
        {
            canvasGroup.alpha = 1;
            StartCoroutine( SplashRoutine(AfterSplash) );
        }

        private IEnumerator SplashRoutine (UnityAction AfterSplash)
        {
            yield return new WaitForSecondsRealtime(silenceDuration);

            //logo animation
            logo.enabled = true;

            yield return new WaitForSecondsRealtime(logoDuration);

            AfterSplash?.Invoke();
            canvasGroup.alpha = 0;
        }
    }
}
