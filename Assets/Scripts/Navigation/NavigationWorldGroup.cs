using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class NavigationWorldGroup : MonoBehaviour
{
    [SerializeField] Vector2 spawnPoint;
    [SerializeField] Vector2 returnPoint;
    
    [Header("Colors")]
    public Color BackgroundColor;
    public Color SelectedColor;
    public Color UnselectedColor;
    public Color UnavailableColor;

    [Header("References")]
    [SerializeField] Image backgroundImage;
    [SerializeField] PolygonCollider2D cameralWall;
    [SerializeField] CinemachineConfiner confiner;

    public Vector2 SpawnPoint 
    { 
        get { return (Vector2)transform.position + spawnPoint; } 
    }

    public Vector2 ReturnPoint 
    { 
        get { return (Vector2)transform.position + returnPoint; } 
    }

    public void SetActive (bool value)
    {
        if (value && confiner && cameralWall)
            confiner.m_BoundingShape2D = cameralWall;

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(SpawnPoint,  radius: .1f);
        Gizmos.DrawWireSphere(ReturnPoint, radius: .1f);
    }
}
