using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagerScreen : MonoBehaviour
{
    [SerializeField] int initialIndex = -1;
    [SerializeField] List <PagerInteractable> interactables;

    List<PagerInteractable> activeInteractables;

    int index = -1;

    void OnEnable()
    {
        activeInteractables = new List<PagerInteractable>();
        foreach (PagerInteractable interactable in interactables)
        {
            if (interactable.gameObject && interactable.gameObject.activeSelf)
                activeInteractables.Add (interactable);
        }

        if (activeInteractables.Count < 1)
        {
            gameObject.SetActive(false);
            return;
        }

        if (index < 0)
            index = (initialIndex > -1) ? initialIndex : 0;

        SelectOnIndex();
    }

    private void SelectOnIndex()
    {
        for (int i = 0; i < interactables.Count; i++)
            interactables[i].Deselect();
        
        activeInteractables[index % activeInteractables.Count].Select();
    }

    public void ChangeIndex (int change)
    {
        index += change;
        if (index < 0)
            index = activeInteractables.Count - 1;
        index %= activeInteractables.Count;

        SelectOnIndex();
    }

    public void ClickInput()
    {
        activeInteractables[index % activeInteractables.Count].OnClick();
    }

    public bool HorizontalInput (float value)
    {
        return activeInteractables[index % activeInteractables.Count].OnHorizontalInput(value);
    }
}
