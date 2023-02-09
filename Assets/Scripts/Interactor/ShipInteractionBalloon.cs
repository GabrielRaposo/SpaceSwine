using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ShipInteractionBalloon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SpriteRenderer boxOutlineRenderer;
    [SerializeField] SpriteRenderer boxShapeRenderer;
    [SerializeField] TextMeshPro textDisplay;

    [Header("Tween Values")]
    [SerializeField] float duration;

    Sequence sequence;

    public void SetInstantState (bool value)
    {
        boxOutlineRenderer.enabled = value;
        boxShapeRenderer.enabled = value;
        textDisplay.enabled = value;
    }

    public void SetTextDisplay (ShipAction shipAction)
    {
        switch(shipAction)
        {
            case ShipAction.Talk:
                textDisplay.text = LocalizationManager.GetUiText("talk", "Talk--");
                break;

            case ShipAction.Navigate:
                textDisplay.text = LocalizationManager.GetUiText("navigate", "navigate--");
                break;

            case ShipAction.Chill:
                textDisplay.text = LocalizationManager.GetUiText("chill", "chill--");
                break;

            case ShipAction.GGS:
                textDisplay.text = LocalizationManager.GetUiText("play", "play--");
                break;

            case ShipAction.Customize:
                textDisplay.text = LocalizationManager.GetUiText("customize", "customize--");
                break;
                
            case ShipAction.Leave:
                textDisplay.text = LocalizationManager.GetUiText("leave", "Leave");
                break;

            default:
                break;
        }

        float boxLenght = .4f + (textDisplay.text.Length * .15f);
        boxOutlineRenderer.size = boxShapeRenderer.size = new Vector2(boxLenght, .5f);
    }

    public void SetHighlight (bool value)
    {
        SetInstantState(true);    

        if (sequence != null)
            sequence.Kill();

        sequence = DOTween.Sequence();
        
        transform.localScale = value ? Vector3.zero : Vector3.one * 1.5f; 

        sequence.Append( transform.DOScale(value ? 1.5f : 0, duration) );
    }
}
