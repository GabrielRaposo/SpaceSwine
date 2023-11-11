using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationWorldGroup : MonoBehaviour
{
    [SerializeField] Vector2 spawnPoint;
    [Space(10)]
    [SerializeField] Image backgroundImage;
    public Color BackgroundColor;
    public Color SelectedColor;
    public Color UnselectedColor;
    public Color UnavailableColor;

    public Vector2 SpawnPoint 
    { 
        get { return (Vector2)transform.position + spawnPoint; } 
    }

    public void SetActive (bool value)
    {
        gameObject.SetActive (value);
    }

    private void OnValidate()
    {
        if (backgroundImage)
            backgroundImage.color = BackgroundColor;

        NavigationObject[] navigationObjects = GetComponentsInChildren<NavigationObject>();
        if (navigationObjects.Length < 1 )
            return;

        foreach (NavigationObject navigationObject in navigationObjects)
        {
            navigationObject.UpdateColors
            (
                navigationObject.UnselectedUsesSelectedColor ? SelectedColor : UnselectedColor, 
                BackgroundColor
            );
        }
    }
}
