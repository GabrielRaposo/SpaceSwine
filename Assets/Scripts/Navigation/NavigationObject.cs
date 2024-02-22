using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using RedBlueGames.Tools.TextTyper;
using UnityEditor;

public class NavigationObject : StoryEventDependent
{
    [SerializeField] private bool blockInteraction;
    
    [Header(" ")]
    [SerializeField] protected SpriteRenderer sprite;
    [SerializeField] SpriteRenderer backSprite;
    [SerializeField] private Color unselectedColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unavailableColor;
    public bool UnselectedUsesSelectedColor;


    [SerializeField] private Canvas _canvas;
    
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI descriptionField;
    [SerializeField] private TextMeshProUGUI coordnatesField;
    [SerializeField] private Image displayLine;

    [Header("Object Info")]
    //[SerializeField] private string displayName;
    //[SerializeField] private string description;
    [SerializeField] private string unavailableNameCode;
    [SerializeField] private string unavailableDescriptionCode;
    [SerializeField] private string nameCode;
    [SerializeField] private string descriptionCode;
    
    [SerializeField] private Vector3 coordinates;

    private Coroutine nameRoutine;
    private Coroutine descriptionRoutine;
    private Coroutine coordinatesRoutine;
    private Coroutine lineRoutine;
    
    protected UnityAction<NavigationShip> interactAction;
    protected NavigationShip ship;

    protected NavigationWorldGroup worldGroup;

    private string DescriptionCode
    {
        get 
        {
            if (string.IsNullOrEmpty(descriptionCode))
                return string.Empty;

            return LocalizationManager.GetUiText(descriptionCode, "???");
        }
    }

    private void Awake()
    {
        worldGroup = GetComponentInParent<NavigationWorldGroup>();

        CloseDisplay();
    }

    protected virtual void OnEnable()
    {
        interactAction = null;
        LocalizationManager.AddToLanguageChangeActionList(QuickForceText);
    }

    protected virtual void OnDisable()
    {
        interactAction = null;
        LocalizationManager.RemoveFromLanguageChangeActionList(QuickForceText);
    }

    public virtual void OnSelect()
    {
        Color unavailable = worldGroup ? worldGroup.UnavailableColor : unavailableColor;
        Color selected = worldGroup ? worldGroup.SelectedColor : selectedColor;
        sprite.color = blockInteraction ? unavailable : selected;
        OpenDisplay();
    }

    public virtual void OnDeselect()
    {
        if (!UnselectedUsesSelectedColor)
            sprite.color = worldGroup ? worldGroup.UnselectedColor : unselectedColor;
        else 
            sprite.color = worldGroup ? worldGroup.SelectedColor : selectedColor;  

        CloseDisplay();
    }

    public void OnInteract(NavigationShip ship = null)
    {
        if (blockInteraction)
            return;

        //Debug.Log("OnInteract()");
        interactAction?.Invoke( ship );
    }

    private void OpenDisplay()
    {
        if (_canvas)
            _canvas.gameObject.SetActive(true);
        StopAllCoroutines();
        
        lineRoutine = StartCoroutine(ExtendLine());
        nameRoutine = StartCoroutine(ShowName());
        descriptionRoutine = StartCoroutine(ShowDescription());
        coordinatesRoutine = StartCoroutine(SetCoordinates());
    }

    private void CloseDisplay()
    {
        if (_canvas)
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
        TextTyper typer = nameField.GetComponent<TextTyper>();
        if (!typer)
            yield break;

        if (typer.IsTyping)
            typer.Skip();

        nameField.text = "";
        typer.TypeText( LocalizationManager.GetUiText(blockInteraction ? unavailableNameCode : nameCode, "???") );

        //int count = displayName.Length;
        //int i = 0;

        //while (i<=count)
        //{
        //    nameField.text = displayName.Substring(0, i);
        //    yield return new WaitForSeconds(0.08f);
        //    i++;
        //}
        //yield break;
    }
    private IEnumerator ShowDescription()
    {
        TextTyper typer = descriptionField.GetComponent<TextTyper>();
        if (!typer)
            yield break;

        if (typer.IsTyping)
            typer.Skip();

        descriptionField.text = "";
        typer.TypeText
        (
            blockInteraction ? 
                LocalizationManager.GetUiText(unavailableDescriptionCode, fallback: "???") :
                DescriptionCode
        );
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

    private void QuickForceText()
    {
        StopAllCoroutines();
        
        if(!string.IsNullOrEmpty(nameField.text))
            nameField.text = LocalizationManager.GetUiText(nameCode, "???");
        
        if(!string.IsNullOrEmpty(descriptionField.text))
            descriptionField.text = DescriptionCode;
    }


    public virtual void SetNotificationIcon() { }

    public void UpdateColors (Color lineColor, Color backColor)
    {
        sprite.color = lineColor;

        if (backSprite)
            backSprite.color = backColor;
    }
}
