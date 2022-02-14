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
        [SerializeField] AK.Wwise.Event logoAKEvent;

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
            logoAKEvent?.Post(gameObject);
            yield return new WaitForSecondsRealtime((logoDuration * 2) / 3);
            logo.enabled = false;
            yield return new WaitForSecondsRealtime((logoDuration * 1) / 3);

            AfterSplash?.Invoke();

            yield return new WaitForSecondsRealtime(1f);
            SetVisibility(false);
        }

        public void SetVisibility(bool value)
        {
            canvasGroup.alpha = value? 1 : 0;
        }

    }
}
