using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBuildEndCaller : Interactable
{
    [SerializeField] GameObject inputIcon;

    private void Start() 
    {
        HighlightState(false);
    }

    public override void Interaction(PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        Debug.Log("Hi icon");
    }

    protected override void HighlightState(bool value) 
    {
        if (inputIcon)
            inputIcon.SetActive(value);
    }

}
