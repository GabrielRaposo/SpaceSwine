using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShipDialogueOptions : MonoBehaviour
{
    [SerializeField] private TitleMenuNavigation titleMenuNavigation;
    [SerializeField] private List<TitleMenuButton> buttons;

    public void InitializeOptions(List<(int, string)> list, UnityAction<int> jumpToNextDialog)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int index = i;
            buttons[index].gameObject.SetActive(true);
            var text = buttons[index].textDisplay;
            text.text = list[index].Item2;
            
            buttons[index].SetEvent(() =>
            {
                CloseDialogOptions();
                jumpToNextDialog.Invoke(list[index].Item1-1);
            });
        }

        titleMenuNavigation.OnFocus = true;
    }

    private void CloseDialogOptions()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].RemoveAllListeners();
            buttons[i].gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

}
