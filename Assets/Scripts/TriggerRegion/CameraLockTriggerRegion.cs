using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraLockTriggerRegion : MonoBehaviour
{
    bool activated;
    CameraFocusController cameraFocusController;
    
    [HideInInspector] public UnityAction onCallAction;

    void Start()
    {
        cameraFocusController = CameraFocusController.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (!collision.CompareTag("Player"))
            return;    

        if (activated)
            return;

        if (cameraFocusController)
        {
            cameraFocusController.SetStaticFocus();
            activated = true;
        }

        if (onCallAction != null)
        {
            onCallAction.Invoke();
        }
    }

    //private void OnTriggerExit2D (Collider2D collision) 
    //{
    //    if (!collision.CompareTag("Player"))
    //        return;

    //    if (tutorialTextBox)
    //    {
    //        tutorialTextBox.HideText();
    //        activated = false;
    //    } 
    //}
}
