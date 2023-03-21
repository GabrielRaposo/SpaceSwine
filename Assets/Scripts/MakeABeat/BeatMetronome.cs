using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace MakeABeat
{ 
    public class BeatMetronome : MonoBehaviour
    {
        const float BLINK_DURATION = 0.1f;

        [SerializeField] AK.Wwise.Event onToggleStateAKEvent;

        [Header("References")]
        [SerializeField] Transform knob;    
        [SerializeField] SpriteRenderer sideButton;
        [SerializeField] SpriteRenderer leftLED;
        [SerializeField] SpriteRenderer rightLED;

        BeatMaster beatMaster;

        void Start()
        {
            beatMaster = GetComponentInParent<BeatMaster>();
            if (!beatMaster)
            {
                gameObject.SetActive(false);
                return;
            }

            beatMaster.CyclePulse_Action     += CyclePulse;
            beatMaster.SignaturePulse_Action += SignaturePulse;
            beatMaster.UpdateDisplay_Action  += UpdateKnob; 

            {
                sideButton.flipY = false;
                leftLED.enabled = rightLED.enabled = false;
            }
        }

        private void CyclePulse()
        {
            rightLED.DOKill();
            rightLED.enabled = true;
            DOVirtual.DelayedCall(BLINK_DURATION, () => rightLED.enabled = false);
        }

        private void SignaturePulse(int signature)
        {
            knob.DOKill();
            knob.localScale = Vector3.one;
            knob.DOPunchScale(Vector3.one * .1f, .3f, vibrato: 2);

            leftLED.DOKill();
            leftLED.enabled = true;
            DOVirtual.DelayedCall(BLINK_DURATION, () => leftLED.enabled = false);
        }

        private void UpdateKnob(float t) 
        {
            knob.localEulerAngles = Vector3.back * Mathf.Lerp(0f, 360f, t);    
            sideButton.flipY = beatMaster.IsRunning;
        }

        public void OnConfirmInput()
        {
            if (!beatMaster)
                return;
            
            if (onToggleStateAKEvent != null)
                onToggleStateAKEvent.Post(gameObject);

            beatMaster.TogglePlayingState();
            sideButton.flipY = beatMaster.IsRunning;
        }
    }
}