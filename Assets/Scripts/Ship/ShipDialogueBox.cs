using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipDialogueBox : MonoBehaviour
{
    const int BASE_WIDTH  = 302;    
    const int BASE_HEIGHT = 101; 

    [Header("Images")]
    [SerializeField] Image largeBGImage;
    [SerializeField] Image smallBGImage;

    [Header("Rect Transforms")]
    [SerializeField] RectTransform largeBG;
    [SerializeField] RectTransform smallBG;
    [SerializeField] RectTransform sideRivets;
    [SerializeField] RectTransform borderTop;
    [SerializeField] RectTransform borderBottom;

    void Start()
    {
        
    }


}
