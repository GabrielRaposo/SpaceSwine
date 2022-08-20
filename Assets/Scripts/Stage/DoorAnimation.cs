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
    [SerializeField] Color blinkColor;
    [SerializeField] ColorSwapper boxColorSwapper;
    [SerializeField] AK.Wwise.Event enterAKEvent;

    Sequence sequence;

    private void Start() 
    {
        Round round = GetComponentInParent<Round>();
        if (!round)
            return;

        round.OnReset += ResetComponents;
    }

    private void ResetComponents()
    {
        if (sequence != null)
        {
            sequence.Kill();

            if (enterAKEvent != null && enterAKEvent.IsPlaying(gameObject))
                enterAKEvent.Stop(gameObject);

            PlayerCharacter player = PlayerCharacter.Instance;
            if (!player)
                return;

            PlayerCharacter playerCharacter = player.GetComponent<PlayerCharacter>();
            PlayerReferences playerReferences = player.GetComponent<PlayerReferences>();
            PlayerShaderController playerShader = player.GetComponent<PlayerShaderController>();

            playerCharacter.SetPhysicsBody(true);
            playerReferences.backlightPS.Stop();
            playerShader.SetWhiteFade(0f);

            GameManager.BlockCharacterInput = false;
        }

        boxColorSwapper.RestoreColor();
        whiteCircle.enabled = false;
        whiteCircle.color = new Vector4 (1, 1, 1, 0);

        // Resetar as interações com o player também
        /*
            PlayerCharacter playerCharacter = player.GetComponent<PlayerCharacter>();
            PlayerReferences playerReferences = player.GetComponent<PlayerReferences>();
            PlayerShaderController playerShader = player.GetComponent<PlayerShaderController>();
        */
    }

    // -- Chamada de transiçaõ de Safety -> Danger
    public void SetupAnimationDangerZone( Door door, GameObject player, UnityAction OnAnimationStart)
    {
        GameManager.BlockCharacterInput = true;

        if (sequence != null)
            sequence.Kill();

        PlayerCharacter playerCharacter = player.GetComponent<PlayerCharacter>();
        PlayerReferences playerReferences = player.GetComponent<PlayerReferences>();
        PlayerShaderController playerShader = player.GetComponent<PlayerShaderController>();
        SpaceJumper spaceJumper = player.GetComponent<SpaceJumper>();

        if (!playerCharacter || !playerReferences || !playerShader || !spaceJumper)
            return;

        OnAnimationStart.Invoke();
        
        // -- Initial Setup
        {
            playerReferences.backlightPS.Play();

            playerCharacter.SetPhysicsBody(false);
            player.transform.SetParent(door.transform);
            playerShader.SetWhiteFade(0f);
            spaceJumper.CancelOnLaunch();

            door.SetInteractable(false);

            OnEnterAreaPS?.Play();
            enterAKEvent?.Post(gameObject);
        }

        sequence = DOTween.Sequence();

        // -- Aproximação do Player para o centro
        sequence.Append( player.transform.DOMove(transform.position, moveInDuration) );
        sequence.Join( DOVirtual.Float(0f, 1f, moveInDuration - .1f, (f) => playerShader.SetWhiteFade(f) ) );

        // -- Brilho principal
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
        sequence.Join( BlinkBoxSequence() );
        sequence.AppendInterval(.5f);

        // -- Ajuste do player para o próximo round
        sequence.AppendCallback( () =>
            { 
                playerCharacter.SetPhysicsBody(true);
                playerReferences.backlightPS.Stop();
                playerShader.SetWhiteFade(0f);
                spaceJumper.enabled = true;

                whiteCircle.enabled = false;
            }
        );
        sequence.OnComplete
        (
            () => 
            {
                GameManager.BlockCharacterInput = false;
                //OnAnimationStart.Invoke(); 
            }
        );
    }
    
    // -- Chamada de próximo Round
    public void SetupAnimationStageTransiton( Door door, GameObject player, UnityAction OnAnimationEnd)
    {
        GameManager.BlockCharacterInput = true;

        if (sequence != null)
            sequence.Kill();

        PlayerCharacter playerCharacter = player.GetComponent<PlayerCharacter>();
        PlayerReferences playerReferences = player.GetComponent<PlayerReferences>();
        PlayerShaderController playerShader = player.GetComponent<PlayerShaderController>();
        SpaceJumper spaceJumper = player.GetComponent<SpaceJumper>();

        if (!playerCharacter || !playerReferences || !playerShader)
            return;

        // -- Initial Setup
        {
            playerReferences.backlightPS.Play();

            playerCharacter.SetPhysicsBody(false);
            player.transform.SetParent(door.transform);
            playerShader.SetWhiteFade(0f);
            spaceJumper.CancelOnLaunch();

            door.SetInteractable(false);

            OnEnterAreaPS?.Play();
            enterAKEvent?.Post(gameObject);
        }

        sequence = DOTween.Sequence();

        // -- Aproximação do Player para o centro
        sequence.Append( player.transform.DOMove(transform.position, moveInDuration) );
        sequence.Join( DOVirtual.Float(0f, 1f, moveInDuration - .1f, (f) => playerShader.SetWhiteFade(f) ) );

        // -- Brilho principal
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
        sequence.Join( BlinkBoxSequence() );
        sequence.AppendInterval(.5f);

        // -- Ajuste do player para o próximo round
        sequence.AppendCallback( () =>
            { 
                playerCharacter.SetPhysicsBody(true);
                playerReferences.backlightPS.Stop();
                playerShader.SetWhiteFade(0f);

                whiteCircle.enabled = false;
            }
        );
        sequence.OnComplete
        (
            () => 
            {
                GameManager.BlockCharacterInput = false;
                OnAnimationEnd.Invoke(); 
            }
        );
    }

    private Sequence BlinkBoxSequence()
    {
        float t = .075f;
        Sequence s = DOTween.Sequence();

        for (int i = 0; i < 4; i++) 
        {
            s.AppendCallback( () => boxColorSwapper.SetColor(blinkColor) );
            s.AppendInterval( t );

            s.AppendCallback( () => boxColorSwapper.RestoreColor() );
            s.AppendInterval( t );
        }

        return s;
    }
}
