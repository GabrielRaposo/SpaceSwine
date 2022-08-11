using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NavigationObject : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer sprite;
    [FormerlySerializedAs("selectionColor")] [SerializeField] private Color unselectedColor;
    [FormerlySerializedAs("selectionColor")] [SerializeField] private Color selectedColor;


    [SerializeField] private Canvas _canvas;
    
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI descriptionField;
    [SerializeField] private TextMeshProUGUI coordnatesField;
    [SerializeField] private Image displayLine;

    [Header("Object Info")]
    [SerializeField] private string displayName;
    [SerializeField] private string description;
    [SerializeField] private Vector3 coordinates;

    private Coroutine nameRoutine;
    private Coroutine descriptionRoutine;
    private Coroutine coordinatesRoutine;
    private Coroutine lineRoutine;
    
    protected UnityAction<NavigationShip> interactAction;
    protected NavigationShip ship;

    private void Awake()
    {
        CloseDisplay();
    }

    public virtual void OnSelect()
    {
        sprite.color = selectedColor;
        OpenDisplay();
    }

    public virtual void OnDisselect()
    {
        sprite.color = unselectedColor;
        CloseDisplay();
    }

    public void OnInteract(NavigationShip ship = null)
    {
        Debug.Log("OnInteract()");
        interactAction?.Invoke(ship);
    }

    private void OpenDisplay()
    {
        _canvas.gameObject.SetActive(true);
        StopAllCoroutines();
        
        lineRoutine = StartCoroutine(ExtendLine());
        nameRoutine = StartCoroutine(ShowName());
        descriptionRoutine = StartCoroutine(ShowDescription());
        coordinatesRoutine = StartCoroutine(SetCoordinates());
    }

    private void CloseDisplay()
    {
        _canvas.gameObject.SetActive(false);
        //StopAllCoroutines();
    }
    
    private IEnumerator ExtendLine()
    {
        displayLine.transform.localScale = new Vector3(0, 1f, 1f);
        while (displayLine.transform.localScale.x < 1f)
        {
            displayLine.transform.localScale = new Vector3(displayLine.transform.localScale.x + 0.1f, 1f, 1f);
            yield return new WaitForSeconds(0.04f);    
        }
    }

    private IEnumerator ShowName()
    {
        nameField.text = "";

        int count = displayName.Length;
        int i = 0;

        while (i<=count)
        {
            nameField.text = displayName.Substring(0, i);
            yield return new WaitForSeconds(0.08f);
            i++;
        }
    }
    private IEnumerator ShowDescription()
    {
        descriptionField.text = "";

        int count = description.Length;
        int i = 0;

        while (i<=count)
        {
            descriptionField.text = description.Substring(0, i);
            yield return new WaitForSeconds(0.06f);
            i++;
        }
    }

    private IEnumerator SetCoordinates()
    {
        SetCoordinateDisplay(Vector3.zero);

        Vector3 current = Vector3.zero;

        float t = 2.2f;


        DOTween.To(() => current
            , value => current = value
            , coordinates, t)
            .SetEase(Ease.OutCirc);
        
        while (t>0f)
        {
            SetCoordinateDisplay(current);
            t -= Time.deltaTime;
            yield return null;
        }
        
        SetCoordinateDisplay(coordinates);
        
    }

    private void SetCoordinateDisplay(Vector3 value)
    {
        coordnatesField.text = $"X {value.x:0000.00};Y {value.y:0000.00};Z {value.z:0000.00}";
    }
    
    
}
