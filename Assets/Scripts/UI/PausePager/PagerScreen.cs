using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagerScreen : MonoBehaviour
{
    [SerializeField] List <PagerInteractable> interactables;

    int index = -1;

    void OnEnable()
    {
        if (interactables.Count < 1)
        {
            gameObject.SetActive(false);
            return;
        }

        if (index < 0)
            index = 0;

        SelectOnIndex();
    }

    private void SelectOnIndex()
    {
        for (int i = 0; i < interactables.Count; i++)
            interactables[i].Deselect();
        
        interactables[index % interactables.Count].Select();
    }

    public void ChangeIndex (int change)
    {
        index += change;
        if (index < 0)
            index = interactables.Count - 1;
        index %= interactables.Count;

        SelectOnIndex();
    }

    public void ClickInput()
    {
        interactables[index % interactables.Count].OnClick();
    }

    public void HorizontalInput (float value)
    {
        interactables[index % interactables.Count].OnHorizontalInput(value);
    }
}
