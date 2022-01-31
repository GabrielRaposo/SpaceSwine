using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CustomCameraTriggerRegion : MonoBehaviour
{
    [SerializeField] float cameraSize;
    [SerializeField] float duration;

    bool activated;
    float originalSize;
    //CameraSizeController cameraSizeController;
    //CameraFocusController cameraFocusController;

    Sequence mainSequence;

    private void Start() 
    {
        originalSize = Camera.main.orthographicSize;
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (!collision.CompareTag("Player"))
            return;    

        if (activated)
            return;

        //originalSize = CameraSizeController.Size;
        //originalSize = Camera.main.orthographicSize;

        if (mainSequence != null)
            mainSequence.Kill();
        mainSequence = DOTween.Sequence();
        mainSequence.Append
        (
            DOVirtual.Float( originalSize, cameraSize, duration, (f) => CameraSizeController.Size = f)
        );

        activated = true;
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        if (!collision.CompareTag("Player"))
            return;

        if (!activated)
            return;

        float startingSize = Camera.main.orthographicSize;

        if (mainSequence != null)
            mainSequence.Kill();
        mainSequence = DOTween.Sequence();
        mainSequence.Append
        (
            DOVirtual.Float( startingSize, originalSize, duration, (f) => CameraSizeController.Size = f)
        );

        activated = false;
    }
}
