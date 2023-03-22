using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MakeABeat
{
    public class BeatMaster : MonoBehaviour
    {
        const int TIME_SIGNATURE = 8;

        [SerializeField] float BPM;

        [Header("Test Vars")]
        [SerializeField] AK.Wwise.Event testCycleAkEvent;
        [SerializeField] AK.Wwise.Event testTickAkEvent;

        [HideInInspector] public UnityAction        StopAll_Action; 
        [HideInInspector] public UnityAction        CyclePulse_Action; 
        [HideInInspector] public UnityAction<int>   SignaturePulse_Action; 
        [HideInInspector] public UnityAction<float> UpdateDisplay_Action; 

        float totalTime;
        float beatTime;

        float timeCount;
        int signatureCount;

        public bool IsRunning { get; private set; }

        void Awake()
        {
            Application.targetFrameRate = 120;

            if (BPM > 0)
                beatTime = 60 / BPM;

            if (beatTime > 0)
                totalTime = beatTime * TIME_SIGNATURE;

            if (UpdateDisplay_Action != null)
                UpdateDisplay_Action.Invoke(0);

            //this.WaitSeconds( 3, StartCycle );
        }

        public bool StartCycle()
        {
            if (IsRunning)
                return false;

            timeCount = 0;
            ResetCycle();

            return true;
        }

        private void ResetCycle()
        {
            //if (testCycleAkEvent != null)
            //    testCycleAkEvent.Post(gameObject);

            // -- chama eventos de som
            if (CyclePulse_Action != null)
                CyclePulse_Action.Invoke();

            if (UpdateDisplay_Action != null)
                UpdateDisplay_Action.Invoke(timeCount / totalTime);

            signatureCount = 1;
            IsRunning = true;
        }

        public void StopCycle()
        {
            if (StopAll_Action != null)
                StopAll_Action.Invoke();

            timeCount = 0;
            if (UpdateDisplay_Action != null)
                UpdateDisplay_Action.Invoke(timeCount / totalTime);

            signatureCount = 1;
            IsRunning = false;
        }

        void Update()
        {
            if (!IsRunning)
                return;

            // -- Chama pulso de tween no display a cada TIME_SIGNATURE tempos
            if (timeCount > beatTime * signatureCount)
            {
                if (SignaturePulse_Action != null)
                    SignaturePulse_Action.Invoke(signatureCount - 1);

                //if (testTickAkEvent != null)
                //    testTickAkEvent.Post(gameObject);

                signatureCount++;
                if (signatureCount > TIME_SIGNATURE)
                    signatureCount = 1;
            }

            // -- Ponto de reset do ciclo 
            if (timeCount >= totalTime)
            {
                ResetCycle();
                timeCount -= totalTime;
            }

            // --  Soma o tempo em timeCount
            timeCount += Time.unscaledDeltaTime;

            if (UpdateDisplay_Action != null)
                UpdateDisplay_Action.Invoke(timeCount / totalTime);
        }

        public void TogglePlayingState()
        {
            if (IsRunning)
                StopCycle();
            else
                StartCycle();
        }

        public float GetTimePerBeat()
        {
            if (beatTime <= 0)
                return -1;

            return (timeCount % beatTime) / beatTime;
        }
    }
}
