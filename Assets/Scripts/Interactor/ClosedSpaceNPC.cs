using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableNPC))]
public class ClosedSpaceNPC : MonoBehaviour
{
    [SerializeField] bool deactivateHelmetOnStart;
    [SerializeField] ClosedSpaceInner closedSpaceInner;
    [SerializeField] SpriteRenderer visualComponent;    
    [SerializeField] SpriteRenderer helmetVisualComponent;

    InteractableNPC interactableNPC;
    Sequence s;

    private void Awake()
    {
        interactableNPC = GetComponent<InteractableNPC>();
    }

    void Start()
    {
        if (!closedSpaceInner || !visualComponent || !helmetVisualComponent)
            return;
        
        { 
            visualComponent.color = new Color (1,1,1,0);
            helmetVisualComponent.color = new Color (1,1,1,0);

            if (deactivateHelmetOnStart)
                SetHelmetState(false);
            
            interactableNPC.SetInteraction(false);
        }

        closedSpaceInner.OnSetStateAction += CallTransition;
    }

    public void CallTransition(bool value, float duration)
    {
        if (s != null)
            s.Kill();

        s = DOTween.Sequence();
        s.Append(visualComponent.DOColor(value ? Color.white : new Color(1,1,1,0), duration));
        s.Join(helmetVisualComponent.DOColor(value ? Color.white : new Color(1,1,1,0), duration));

        s.OnComplete( () => interactableNPC.SetInteraction(value) );
    }

    public void SetHelmetState(bool value)
    {
        if (!helmetVisualComponent)
            return;

        helmetVisualComponent.enabled = value;
    }
}
