using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagerAxisButtonsVisual : MonoBehaviour
{
    [SerializeField] GameObject topButton;
    [SerializeField] GameObject lowerButton;
    [SerializeField] GameObject rightButton;
    [SerializeField] GameObject leftButton;
    
    void Start()
    {
        if (!topButton || !lowerButton || !rightButton || !leftButton)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    public void ReadOnline (Vector2 axis)
    {
        topButton.SetActive (axis.y <= 0);
        lowerButton.SetActive (axis.y >= 0);
        rightButton.SetActive (axis.x <= 0);
        leftButton.SetActive (axis.x >= 0);
    }
}
