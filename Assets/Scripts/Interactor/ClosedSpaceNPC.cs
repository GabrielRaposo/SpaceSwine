using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableNPC))]
public class ClosedSpaceNPC : MonoBehaviour
{
    [SerializeField] ClosedSpaceInner closedSpaceInner;
    [SerializeField] SpriteRenderer visualComponent;    

    InteractableNPC interactableNPC;
    Sequence s;

    private void Awake()
    {
        interactableNPC = GetComponent<InteractableNPC>();
    }

    void Start()
    {
        if (!closedSpaceInner || !visualComponent)
            return;
        
        visualComponent.color = new Color (1,1,1,0);
        interactableNPC.SetInteraction(false);

        closedSpaceInner.OnSetStateAction += CallTransition;
    }

    public void CallTransition(bool value, float duration)
    {
        if (s != null)
            s.Kill();

        s = DOTween.Sequence();
        s.Append(visualComponent.DOColor(value ? Color.white : new Color(1,1,1,0), duration));

        s.OnComplete( () => interactableNPC.SetInteraction(value) );
    }

}
