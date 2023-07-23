using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

public class AdventureLogTab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textDisplay;

    [Header("Sequence")]
    [SerializeField] Gradient flashGradient;
    [SerializeField] float recolorDuration;
    [SerializeField] float slideDuration;
    [SerializeField] float shrinkDuration;

    Sequence sequence;

    public AdventureLogScriptableObject data { get; private set; }

    public void Setup (AdventureLogScriptableObject data)
    {
        this.data = data;

        if (textDisplay)
        {
            string localizedtext = LocalizationManager.GetUiText(data.logTextID, data.fallbackText);
            textDisplay.text = localizedtext;
        }

        SetActiveState(true);
    }

    public void SetActiveState (bool value)
    {
        gameObject.SetActive(value);
    }

    public void StrikeAndVanish (UnityAction afterVanish)
    {
        if (sequence != null)
            sequence.Kill();

        sequence = DOTween.Sequence();

        sequence.AppendInterval(.1f);
        sequence.AppendCallback ( () => textDisplay.text = "<s>" + textDisplay.text );
        sequence.Append( transform.DOPunchScale(Vector3.one * .1f, .2f, vibrato: 2 ) );
        
        float reps = 7;
        for (int i = 0; i < reps; i++)
        {
            sequence.Append
            ( 
                DOVirtual.Float (0.0f, 1.0f, recolorDuration / reps, 
                    (f) => 
                    {
                        textDisplay.color = flashGradient.Evaluate(f);
                    })
            );
        }

        sequence.Append 
        (
            DOVirtual.Float (0, 1000, slideDuration,
                (f) => 
                {
                    textDisplay.rectTransform.MoveX(f);
                }).SetEase(Ease.InCirc)
        );

        RectTransform rt = GetComponent<RectTransform>();
        float startingHeight = rt.sizeDelta.y;
        sequence.Append
        (
            DOVirtual.Float (startingHeight, 0, shrinkDuration,
                (y) => 
                {
                    rt.sizeDelta = new Vector2 ( rt.sizeDelta.x, y );
                })
        );

        sequence.SetUpdate(isIndependentUpdate: true);
        sequence.OnComplete( () => afterVanish.Invoke() );
    }

}
