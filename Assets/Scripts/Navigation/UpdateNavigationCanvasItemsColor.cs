using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateNavigationCanvasItemsColor : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> textDisplays;
    [SerializeField] List<Image> imageDisplays;

    private void OnValidate()
    {
        if (!Application.isEditor)
            return;

        NavigationWorldGroup worldGroup = GetComponentInParent<NavigationWorldGroup>();
        if (worldGroup == null) 
            return;

        if (textDisplays != null && textDisplays.Count > 0)
        {
            foreach (var textDisplay in textDisplays) 
            {
                textDisplay.color = worldGroup.SelectedColor;   
            }
        }

        if (imageDisplays != null && imageDisplays.Count > 0)
        {
            foreach (var imageDisplay in imageDisplays) 
            {
                imageDisplay.color = worldGroup.SelectedColor;   
            }
        }

    }
}
