using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using RedBlueGames.Tools.TextTyper;

namespace WhitethornBuild
{
    public class BuildOutroCardDisplay : MonoBehaviour
    {
        [SerializeField] BuildEndInputListener buildInputListener;

        [Header("Values")]
        [SerializeField] float fadeInDuration;
        [SerializeField] float inputDelayDuration;
        [SerializeField] string thanksTextCode;
        [SerializeField] string inputTextCode;
        [SerializeField] string parseSeparator;

        [Header("References")]
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] TextMeshProUGUI textDisplay;
        [SerializeField] TextTyper textTyper;
        [SerializeField] TextMeshProUGUI inputTextDisplay;

        private void Start() 
        {
            if (canvasGroup)
                canvasGroup.alpha = 0;

            if (textDisplay)
                textDisplay.enabled = false;
        }

        public void CallScreen()
        {
            if (!canvasGroup || !textDisplay || !textTyper || !inputTextDisplay)
                return;

            canvasGroup.alpha = 0;
        
            // -- Thanks Text
            string originalText = textDisplay.text;
            { 
                (bool isValid, string output) data = LocalizationManager.GetStoryText(thanksTextCode);
                if (data.isValid)
                    originalText = data.output;

                textDisplay.text = string.Empty;
                textDisplay.enabled = true;
            }

            // -- Input Text
            {
                string inputText = inputTextDisplay.text;
                (bool isValid, string output) data = LocalizationManager.GetStoryText(inputTextCode);
                if (data.isValid)
                    inputText = data.output;
                inputText = ParseInputTag.ParsedOutput(inputText, parseSeparator);

                inputTextDisplay.text = inputText;
                inputTextDisplay.enabled = false;
            }

            Sequence s = DOTween.Sequence();
            s.Append( canvasGroup.DOFade( 1, fadeInDuration ) );
            s.AppendCallback( () => textTyper.TypeText(originalText) );
        }

        public void OnTypeCompleted()
        {
            Sequence s = DOTween.Sequence();
            s.AppendInterval( inputDelayDuration );
            s.AppendCallback( () => inputTextDisplay.enabled = true );
            s.AppendCallback( () => buildInputListener.enabled = true );
        }
    }

}