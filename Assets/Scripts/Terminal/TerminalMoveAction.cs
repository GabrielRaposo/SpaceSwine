using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TerminalMoveAction : MonoBehaviour, ITerminalEvent
{
    [SerializeField] List <Vector2> targetPositions;
    [SerializeField] float duration;

    int index;
    Sequence sequence;
    List<Interactable> interactableChildren; 

    void Start()
    {
        interactableChildren = new List<Interactable>();
        GetComponentsInChildren<Interactable>(interactableChildren);
        Debug.Log("interactableChildren.Count: "  + interactableChildren.Count);

        if (targetPositions.Count < 1)
        {
            enabled = false;
            return;
        }

        index = 0;
        //transform.eulerAngles = Vector3.forward * targetAngles[index];
        transform.position = targetPositions[index];
    }

    public void Activate (InteractableTerminal terminal, PlayerInteractor player)
    {
        index = (index + 1) % targetPositions.Count;

        if (sequence != null)
            sequence.Kill();

        BeforeSequence(player);

        sequence = DOTween.Sequence();
        //sequence.Append( transform.DORotate(targetPositions[index] * Vector3.forward, duration, RotateMode.FastBeyond360) );
        sequence.Append(transform.DOMove(targetPositions[index], duration));
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
