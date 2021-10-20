using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TerminalRotateAction : MonoBehaviour, ITerminalEvent
{
    [SerializeField] List <float> targetAngles;
    [SerializeField] float duration;

    int index;

    void Start()
    {
        if (targetAngles.Count < 1)
        {
            enabled = false;
            return;
        }

        index = 0;
        transform.eulerAngles = Vector3.forward * targetAngles[index];
    }

    public void Activate (InteractableTerminal terminal)
    {
        index = (index + 1) % targetAngles.Count;
        transform.DORotate(targetAngles[index] * Vector3.forward, duration, RotateMode.FastBeyond360);
        // libera inputs após rotação
    }
}
