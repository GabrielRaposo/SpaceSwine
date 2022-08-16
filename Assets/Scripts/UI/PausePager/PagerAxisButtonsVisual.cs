using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagerAxisButtonsVisual : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event clickAKEvent;

    [Header("Buttons")]
    [SerializeField] GameObject topButton;
    [SerializeField] GameObject lowerButton;
    [SerializeField] GameObject rightButton;
    [SerializeField] GameObject leftButton;
    
    Vector2 previous;

    void Start()
    {
        if (!topButton || !lowerButton || !rightButton || !leftButton)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    public void SetAxisButtons (Vector2 axis)
    {
        topButton.SetActive (axis.y <= 0);
        lowerButton.SetActive (axis.y >= 0);
        rightButton.SetActive (axis.x <= 0);
        leftButton.SetActive (axis.x >= 0);
        
        if (axis != Vector2.zero && axis != previous && clickAKEvent != null)
        {
            clickAKEvent.Post(gameObject);
        }
        previous = axis;
    }
}
