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
                textDisplay.text = "Talk";
                break;

            case ShipAction.Navigate:
                textDisplay.text = "Navigate";
                break;

            case ShipAction.Chill:
                textDisplay.text = "Chill";
                break;

            case ShipAction.GGS:
                textDisplay.text = "Play";
                break;

            case ShipAction.Customize:
                textDisplay.text = "Customize";
                break;
                
            case ShipAction.Leave:
                textDisplay.text = "Leave";
                break;

            default:
                break;
        }

        float boxLenght = .4f + (textDisplay.text.Length * .15f);
        boxOutlineRenderer.size = boxShapeRenderer.size = new Vector2(boxLenght, .7f);
    }

    public void SetHighlight (bool value)
    {
        SetInstantState(true);    

        if (sequence != null)
            sequence.Kill();

        sequence = DOTween.Sequence();
        
        transform.localScale = value ? Vector3.zero : Vector3.one; 

        sequence.Append( transform.DOScale(value ? 1 : 0, duration) );
    }
}
