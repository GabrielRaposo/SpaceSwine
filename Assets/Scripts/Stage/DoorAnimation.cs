using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class DoorAnimation : MonoBehaviour
{
    [SerializeField] float moveInDuration;

    Sequence sequence;

    public void SetupAnimation( Door door, GameObject player, UnityAction OnAnimationEnd )
    {
        // TO-DO: Ativar uma variável global de ON_TRANSITION para evitar inputs de pausa e etc 

        if (sequence != null)
            sequence.Kill();

        PlayerCharacter playerCharacter = player.GetComponent<PlayerCharacter>();

        // -- Initial Setup
        {
            if (!playerCharacter)
                return;

            playerCharacter.SetPhysicsBody(false);
            player.transform.SetParent(door.transform);

            door.SetInteractable(false);
        }

        sequence = DOTween.Sequence();

        sequence.Append( player.transform.DOMove(transform.position, moveInDuration) );

        sequence.AppendCallback( () => playerCharacter.SetPhysicsBody(true) );
        sequence.OnComplete( OnAnimationEnd.Invoke );
    }

}
