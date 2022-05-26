using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class DoorAnimation : MonoBehaviour
{
    [SerializeField] float moveInDuration;

    [Header("References")]
    [SerializeField] SpriteRenderer whiteCircle;
    [SerializeField] Transform visualComponent;
    [SerializeField] ParticleSystem OnEnterAreaPS;
    [SerializeField] ParticleSystem CenterBurstPS;

    Sequence sequence;

    private void ResetComponents()
    {
        whiteCircle.enabled = false;
        whiteCircle.color = new Vector4(1, 1, 1, 0);
    }

    public void SetupAnimation( Door door, GameObject player, UnityAction OnAnimationEnd )
    {
        // TO-DO: Ativar uma variável global de ON_TRANSITION para evitar inputs de pausa e etc 

        if (sequence != null)
            sequence.Kill();

        PlayerCharacter playerCharacter = player.GetComponent<PlayerCharacter>();
        PlayerReferences playerReferences = player.GetComponent<PlayerReferences>();

        if (!playerCharacter || !playerReferences)
            return;

        // -- Initial Setup
        {
            playerReferences.backlightPS.Play();

            playerCharacter.SetPhysicsBody(false);
            player.transform.SetParent(door.transform);

            door.SetInteractable(false);

            OnEnterAreaPS?.Play();
        }

        sequence = DOTween.Sequence();

        sequence.Append( player.transform.DOMove(transform.position, moveInDuration) );
        sequence.AppendCallback( () => 
            {
                playerCharacter.gameObject.SetActive(false);
                whiteCircle.enabled = true;
                CenterBurstPS?.Play();
            }
        );

        sequence.Append( visualComponent.DOPunchScale(Vector3.one * -.1f, duration: .3f) );
        sequence.Join
        (
            DOVirtual.Float(from: 1.0f, to: 0.0f, duration: .2f, (f) => whiteCircle.color = new Vector4(1, 1, 1, f))
        );
        sequence.AppendInterval(.75f);

        sequence.AppendCallback( () =>
            { 
                playerCharacter.SetPhysicsBody(true);
                playerReferences.backlightPS.Stop();

                whiteCircle.enabled = false;
            }
        );
        sequence.OnComplete( OnAnimationEnd.Invoke );
    }

}
