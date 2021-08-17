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

        dialogBox?.gameObject.SetActive(false);
    }

    public void SetDialog (string speakerName, List<string> dialog)
    {
        if (!dialogBox)
            return;

        StopAllCoroutines();
        StartCoroutine( DialogLoop( speakerName, dialog) );
    }

    IEnumerator DialogLoop(string speakerName, List<string> dialog)
    {
        OnDialog = true;

        for (int i = 0; i < dialog.Count; i++)
        {
            dialogBox.SetDialog( speakerName, dialog[i] );

            yield return new WaitForEndOfFrame();
            yield return new WaitUntil( () => Input.GetKeyDown(KeyCode.Space) );
            yield return new WaitForEndOfFrame();
        }

        dialogBox.EndDialog();
        OnDialog = false;
    }
}
