using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationWorldGroup : MonoBehaviour
{
    [SerializeField] Vector2 spawnPoint;

    public Vector2 SpawnPoint 
    { 
        get { return (Vector2)transform.position + spawnPoint; } 
    }

    public void SetActive (bool value)
    {
        gameObject.SetActive (value);
    }

}
