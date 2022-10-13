using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MakeABeat
{
    public class TrackTimeDisplay : MonoBehaviour
    {
        [SerializeField] Image fillDisplay; 

        void Start()
        {
            BeatMaker beatMaker = GetComponentInParent<BeatMaker>();
            if (!beatMaker)
            {
                gameObject.SetActive(false);
                return;
            }

            beatMaker.UpdateDisplayAction  += UpdateDisplay;
            beatMaker.SignaturePulseAction += PulseImage;
        }

        void UpdateDisplay (float fillAmount)
        {
            if (!fillDisplay)
                return;

            fillDisplay.fillAmount = 1 - fillAmount;
        }

        void PulseImage(int step)
        {
            
        }
        
    }
}