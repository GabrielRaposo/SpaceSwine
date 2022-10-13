using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MakeABeat
{
    public class TrackTimeDisplay : MonoBehaviour
    {
        [SerializeField] Image fillDisplay; 

        Sequence pulseSequence;

        void Start()
        {
            BeatMaster beatMaster = GetComponentInParent<BeatMaster>();
            if (!beatMaster)
            {
                gameObject.SetActive(false);
                return;
            }

            beatMaster.UpdateDisplay_Action  += UpdateDisplay;
            beatMaster.SignaturePulse_Action += PulseImage;
        }

        void UpdateDisplay (float fillAmount)
        {
            if (!fillDisplay)
                return;

            fillDisplay.fillAmount = 1 - fillAmount;
        }

        void PulseImage(int step)
        {
            if (!fillDisplay)
                return;

            if (pulseSequence != null)
                pulseSequence.Kill();

            RectTransform rt = fillDisplay.rectTransform;
            rt.localScale = Vector3.one; 

            pulseSequence = DOTween.Sequence();
            pulseSequence.Append( rt.DOPunchScale(Vector3.one * -.15f, duration: .25f, vibrato: 1, elasticity: 1 ) );
        }

        private void OnDisable() 
        {
            if (pulseSequence != null)
                pulseSequence.Kill();
        }
    }
}