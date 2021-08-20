using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


[RequireComponent(typeof(Image))]
public class NavigationTab : MonoBehaviour
{
    [SerializeField] Color tabSelectedColor;
    [SerializeField] Color tabDeselectedColor;

    [SerializeField] UnityEvent OnSubmitEvent;

    Image image;

    void Awake()
    {
        image = GetComponent<Image>();    
    }

    private void Start() 
    {
        if (OnSubmitEvent == null)
            OnSubmitEvent = new UnityEvent();
    }

    public void Submit()
    {
        OnSubmitEvent?.Invoke();
    }

    public void Select()
    {
        image.color = tabSelectedColor;
    }

    public void Deselect()
    {
        image.color = tabDeselectedColor;
    }
}
