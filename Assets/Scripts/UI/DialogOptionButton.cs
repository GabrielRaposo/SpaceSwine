using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogOptionButton : InputSystemCompatibleButton
{
    [SerializeField] private Image buttonImage;
    
    public override void Select(bool playSound = false)
    {
        if (playSound && hoverAKEvent != null)
            hoverAKEvent.Post(gameObject);
        
        SetState(true);
        
    }

    public override void Deselect()
    {
        SetState(false);
    }
    
    protected override void SetState(bool value)
    {
        
        textDisplay.color = value? Color.yellow: Color.white;
        
    }

    public override void SetInteraction(bool value)
    {
        if (!value)
            buttonImage.color = new Color(1f, 1f, 1f, 0.4f);
    }


}
