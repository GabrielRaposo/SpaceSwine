using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RobotScreenStatesManager : MonoBehaviour
{
    [SerializeField] Transform screenTransform;

    bool displayed;

    void Start()
    {
        if (!screenTransform)
            return;

        InteractableNPC interactableNPC = GetComponent<InteractableNPC>();
        if (!interactableNPC)
            return;

        screenTransform.localScale = new Vector3(1,0,1);

        interactableNPC.OnInteraction += () => 
        {
            if (displayed)
                return;

            screenTransform.DOScaleY(1f, duration: .1f)
                .SetEase(Ease.Linear);

            displayed = true;
        };
    }
}
