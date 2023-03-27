using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MakeABeat
{
    public class BeatMaster : MonoBehaviour
    {
        const int TIME_SIGNATURE = 4;

        [SerializeField] float BPM;

        [Header("Test Vars")]
        [SerializeField] AK.Wwise.Event testCycleAkEvent;
        [SerializeField] AK.Wwise.Event testTickAkEvent;

        [HideInInspector] public UnityAction        CyclePulse_Action; 
        [HideInInspector] public UnityAction<int>   SignaturePulse_Action; 
        [HideInInspector] public UnityAction<float> UpdateDisplay_Action; 

        float totalTime;
        float loopTime;

        float timeCount;
        int signatureCount;

        bool isRunning;

        void Awake()
        {
            Application.targetFrameRate = 120;

            if (BPM > 0)
                loopTime = 60 / BPM;

            if (loopTime > 0)
                totalTime = loopTime * TIME_SIGNATURE;

            if (UpdateDisplay_Action != null)
                UpdateDisplay_Action.Invoke(0);

            this.WaitSeconds( 3, StartCycle );
        }

        private void StartCycle()
        {
            timeCount = 0;
            ResetCycle();
        }

        private void ResetCycle()
        {
            if (testCycleAkEvent != null)
                testCycleAkEvent.Post(gameObject);

            // -- chama eventos de som
            if (CyclePulse_Action != null)
                CyclePulse_Action.Invoke();

            if (UpdateDisplay_Action != null)
                UpdateDisplay_Action.Invoke(timeCount / totalTime);

            signatureCount = 1;
            isRunning = true;
        }

        public void StopCycle()
        {
            // -- cancela eventos de som

            timeCount = 0;
            signatureCount = 1;
            isRunning = false;
        }

        void Update()
        {
            if (!isRunning)
                return;

            // -- Chama pulso de tween no display a cada TIME_SIGNATURE tempos
            if (timeCount > loopTime * signatureCount)
            {
                if (SignaturePulse_Action != null)
                    SignaturePulse_Action.Invoke(signatureCount - 1);

                if (testTickAkEvent != null)
                    testTickAkEvent.Post(gameObject);

                signatureCount++;
                if(signatureCount > TIME_SIGNATURE)
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
    }
}
