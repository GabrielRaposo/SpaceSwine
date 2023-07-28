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
    [SerializeField] private List<Button> buttons;

    public void InitializeOptions(List<(int, string)> list, UnityAction<int> jumpToNextDialog)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int index = i;
            buttons[index].gameObject.SetActive(true);
            var text = buttons[index].GetComponentInChildren<TMP_Text>();
            text.text = list[index].Item2;
            
            buttons[index].onClick.AddListener(() =>
            {
                CloseDialogOptions();
                jumpToNextDialog.Invoke(list[index].Item1-1);
            });
        }

        StartCoroutine(DebugPress2());
    }

    private void CloseDialogOptions()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public IEnumerator DebugPress2()
    {
        Debug.Log("Start debug");
        yield return new WaitForSeconds(1.5f);
        buttons[0].onClick.Invoke();
    }
    
}
