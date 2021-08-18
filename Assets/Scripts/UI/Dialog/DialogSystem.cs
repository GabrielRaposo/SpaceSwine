using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    [SerializeField] DialogBox dialogBox;

    public static bool OnDialog;
    public static DialogSystem Instance;

    void Awake()
    {
        Instance = this;
    }

    public void SetDialog (string speakerName, List<string> dialog)
    {
        if (!dialogBox)
            return;

        dialogBox.SetDialogData(speakerName, dialog);
    }

    private void OnDisable() 
    {
        OnDialog = false;    
    }
}
