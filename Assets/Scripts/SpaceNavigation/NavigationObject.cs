using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NavigationObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color selectionColor;

    protected UnityAction interactAction;

    public void OnSelect()
    {
        sprite.color = Color.white;
    }

    public void OnDisselect()
    {
        sprite.color = selectionColor;
    }

    public void OnInteract()
    {
        Debug.Log("OnInteract()");
        interactAction?.Invoke();
    }
    
    
}
