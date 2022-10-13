using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MakeABeat
{
    public class BeatMaker : MonoBehaviour
    {
        const int TIME_SIGNATURE = 4;

        // Loop display
        [SerializeField] float BPM;

        [Header("Temp")]
        [SerializeField] int quantTracks;
        [SerializeField] AK.Wwise.Event testAkEvent;

        [HideInInspector] public UnityAction<float>  UpdateDisplayAction; 
        [HideInInspector] public UnityAction<int>    SignaturePulseAction; 

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

            if (UpdateDisplayAction != null)
                UpdateDisplayAction.Invoke(0);

            this.WaitSeconds( 3, StartRun );
            //this.WaitSeconds( 3, () => StartCoroutine(TimerLoop()) );
        }

        private void StartRun()
        {
            // -- chama eventos de som

            if (testAkEvent != null)
                testAkEvent.Post(gameObject);

            timeCount = 0;
            if (UpdateDisplayAction != null)
                UpdateDisplayAction.Invoke(timeCount / totalTime);

            signatureCount = 1;
            isRunning = true;
        }

        private void StopRun()
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

            if (timeCount > loopTime * signatureCount)
            {
                if (SignaturePulseAction != null)
                    SignaturePulseAction.Invoke(signatureCount - 1);

                signatureCount++;
                if(signatureCount > TIME_SIGNATURE)
                    signatureCount = 1;
            }

            if (timeCount >= totalTime)
            //if ( Mathf.Abs(timeCount - totalTime) < Time.unscaledDeltaTime * .9f)
            {
                float t = timeCount - totalTime;

                signatureCount = 1;
                StartRun();
                
                timeCount = t;
                Debug.Log("t: " + t);
            }

            timeCount += Time.unscaledDeltaTime;

            if (UpdateDisplayAction != null)
                UpdateDisplayAction.Invoke(timeCount / totalTime);

            //Debug.Log("timeCount: " + timeCount);
        }

        IEnumerator TimerLoop ()
        {
            while (true)
            {
                if (testAkEvent != null)
                    testAkEvent.Post(gameObject);
                yield return new WaitForSecondsRealtime(totalTime);
            }
        }
    }
}
