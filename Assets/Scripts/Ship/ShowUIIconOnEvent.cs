using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowUIIconOnEvent : StoryEventDependent
{
    [SerializeField] Image secretImage;
    [SerializeField] Image iconImage;
    [SerializeField] StoryEventScriptableObject criteria;

    void Start()
    {
        CallDependentAction (Setup);
    }

    void Setup()
    {
        if (!secretImage || !iconImage)
            return;

        bool state = StoryEventsManager.IsComplete (criteria);
        secretImage.enabled = !state;
        iconImage.enabled = state;
    }
}