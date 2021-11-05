using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TerminalRotateAction : MonoBehaviour, ITerminalEvent
{
    [SerializeField] List <float> targetAngles;
    [SerializeField] float duration;

    int index;
    Sequence sequence;
    List<Interactable> interactableChildren; 

    void Start()
    {
        interactableChildren = new List<Interactable>();
        GetComponentsInChildren<Interactable>(interactableChildren);
        Debug.Log("interactableChildren.Count: "  + interactableChildren.Count);

        if (targetAngles.Count < 1)
        {
            enabled = false;
            return;
        }

        index = 0;
        transform.eulerAngles = Vector3.forward * targetAngles[index];
    }

    public void Activate (InteractableTerminal terminal, PlayerInteractor player)
    {
        index = (index + 1) % targetAngles.Count;

        if (sequence != null)
            sequence.Kill();

        BeforeSequence(player);

        sequence = DOTween.Sequence();
        sequence.Append( transform.DORotate(targetAngles[index] * Vector3.forward, duration, RotateMode.FastBeyond360) );
        sequence.OnComplete(() => AfterSequence(player));
    }

    private void BeforeSequence(PlayerInteractor player)
    {
        if (interactableChildren != null && interactableChildren.Count > 0)
        {
            foreach (Interactable i in interactableChildren)
                i.SetInteraction(false);
        }

        if (player)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput) playerInput.enabled = false;

            PlatformerCharacter platformer = player.GetComponent<PlatformerCharacter>();
            if (platformer) platformer.KillInputs();
        }
    }

    private void AfterSequence(PlayerInteractor player)
    {
        if (interactableChildren != null && interactableChildren.Count > 0)
        {
            foreach (Interactable i in interactableChildren)
                i.SetInteraction(true);
        }

        if (player)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput) playerInput.enabled = true;
        }
    }
}
