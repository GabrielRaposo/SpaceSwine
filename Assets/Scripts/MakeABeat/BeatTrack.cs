using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeABeat
{
    public class BeatTrack : MonoBehaviour
    {
        [SerializeField] BeatTapeScriptableObject beatTape;
        
        BeatTapeScriptableObject currentBeatTape;

        int signatureCount;
        bool isRunning;

        void Start()
        {
            BeatMaster beatMaster = GetComponentInParent<BeatMaster>();
            if (!beatMaster)
            {
                gameObject.SetActive(false);
                return;
            }

            beatMaster.CyclePulse_Action += CyclePulse; 
        }

        private void CyclePulse()
        {
            // -- Se já tem uma Tape tocando e ela não terminou ainda, espera ela acabar antes de trocar
            if (currentBeatTape != null)
            {
                signatureCount++;

                if (signatureCount > currentBeatTape.signatureDuration - 1)
                    signatureCount = 0;
                else return;
            }

            if (currentBeatTape != null && currentBeatTape.sampleAKEvent != null)
            {
                gameObject.StopAllEvents();
            }


            currentBeatTape = beatTape;

            if (currentBeatTape == null)
                return;

            if (currentBeatTape.sampleAKEvent != null)
                currentBeatTape.sampleAKEvent.Post(gameObject);
        }

        private void OnDisable() 
        {
             gameObject.StopAllEvents();
        }
    }
}
