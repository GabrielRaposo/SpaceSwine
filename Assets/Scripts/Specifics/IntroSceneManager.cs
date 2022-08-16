using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;

public class IntroSceneManager : MonoBehaviour
{
    [Header("Soundscape")]
    [SerializeField] SoundLoopCaller soundLoopCaller;
    [SerializeField] GoToSceneTriggerRegion exitTriggerRegion;

    [Header("Sequence Values")]
    [SerializeField] float startingYPosition;
    [SerializeField] float startHoldDuration;
    [SerializeField] float moveDownDuration;
    [SerializeField] float startingZoomValue;
    [SerializeField] float zoomOutValue;
    [SerializeField] float zoomOutDuration;
    [SerializeField] float exitHoleDuration;

    [Header("Camera")]
    [SerializeField] CameraFocusController focusController;
    [SerializeField] CinemachineVirtualCamera playerVirtualCam;
    [SerializeField] AK.Wwise.RTPC stuckVolumeRTPC;

    [Header("Player")]
    [SerializeField] PlayerCharacter player;
    [SerializeField] Animator stuckPlayer;

    [Header("Jump Out")]
    [SerializeField] CanvasGroup jumpTutorialGroup;
    [SerializeField] GameObject jumpOutAnimation;
    [SerializeField] ParticleSystem rubblePS;

    InputAction jumpInput; 
    PlayerInputActions inputActions;

    void Start()
    {
        if (!player || !stuckPlayer)
        {
            enabled = false;
            return;
        }

        StartCoroutine( IntroSequence() );
    }

    private void OnEnable() 
    {
        inputActions = new PlayerInputActions();

        jumpInput = inputActions.Player.Jump;
        jumpInput.Enable();
    }

    IEnumerator IntroSequence()
    {
        // -- Setup
        if (soundLoopCaller)
        {
            soundLoopCaller.StartLoop();

            if (exitTriggerRegion)
            {
                exitTriggerRegion.OnTriggerAction += soundLoopCaller.CallFadeOut;
            }
        }

        jumpTutorialGroup.alpha = 0;
        stuckVolumeRTPC.SetGlobalValue(0);

        LensSettings lensSettings = new LensSettings(fov: 1, startingZoomValue, .3f, 1000, 0);
        playerVirtualCam.m_Lens = lensSettings;
        player.transform.position = Vector2.up * startingYPosition;
        focusController.SetInstantPlayerFocus();
        
        player.SetPhysicsBody(false);
        player.SetHiddenState(true);
        player.GetComponent<PlatformerCharacter>().enabled = false;
        GameManager.BlockCharacterInput = true;

        // -- Desce câmera
        yield return new WaitForSeconds( startHoldDuration );

        bool done = false;
        Sequence s = DOTween.Sequence();
        s.Append
        (
            player.transform.DOMoveY
            (
                endValue: stuckPlayer.transform.position.y,
                duration: moveDownDuration
            )
            .SetEase(Ease.OutCirc)
        );
        float delay = moveDownDuration * .4f;
        s.Join
        (
            DOVirtual.Float
            (
                from: 0, to: 100f, moveDownDuration - delay,
                (f) => 
                {
                    stuckVolumeRTPC.SetGlobalValue(f);
                }
            ).SetDelay(delay)
        );
        s.OnComplete( () => done = true );
        yield return new WaitWhile( () => !done );

        yield return new WaitForSeconds(1.0f);

        // -- Zoom-out (1)
        done = false;
        s = DOTween.Sequence();
        s.Append( 
            player.transform.DOMoveY(1.82f, zoomOutDuration)
                .SetEase(Ease.Linear) 
        );
        s.Join( DOVirtual.Float( startingZoomValue, zoomOutValue, zoomOutDuration, f => 
            { 
                lensSettings = new LensSettings(1, f, .3f, 1000, 0);
                playerVirtualCam.m_Lens = lensSettings; 
            }
        ).SetEase(Ease.Linear) );
        s.OnComplete( () => done = true );
        yield return new WaitWhile( () => !done );

        // -- Apresenta tutorial de pulo
        done = false;
        s = DOTween.Sequence();
        s.Append( jumpTutorialGroup.DOFade(1, 1.0f).SetEase(Ease.Linear) );
        s.OnComplete( () => done = true );
        yield return new WaitWhile( () => !done );

        // -- Lê o input de "Jump"
        float jumpValue; 
        while (true)
        {
            jumpValue = jumpInput.ReadValue<float>();
            if (jumpValue > 0)
                break;
            
            yield return new WaitForEndOfFrame();
        }
        
        jumpTutorialGroup.alpha = 0;
        yield return new WaitForEndOfFrame();

        // -- Animação de pulo do player e Zoom-out (2)
        stuckPlayer.gameObject.SetActive(false);
        jumpOutAnimation.SetActive(true);
        rubblePS.Play();
        RaposUtil.WaitSeconds(this, duration: .4f, () => player.GetComponent<PlatformerCharacter>().enabled = true);

        yield return new WaitForSeconds(exitHoleDuration);
        jumpOutAnimation.SetActive(false);

        // -- Zoom-out (2)
        s = DOTween.Sequence();
        s.Append( DOVirtual.Float( zoomOutValue, to: 3.0f, duration: .5f, f => 
            { 
                lensSettings = new LensSettings(1, f, .3f, 1000, 0);
                playerVirtualCam.m_Lens = lensSettings; 
            }
        ).SetEase(Ease.OutCirc) );

        // -- Libera controle do Player
        stuckPlayer.gameObject.SetActive(false);
        player.SetHiddenState(false);
        player.SetPhysicsBody(true);
        player.GetComponent<PlatformerCharacter>().enabled = true;
        GameManager.BlockCharacterInput = false;
    }

    private void OnDisable() 
    {
        jumpInput.Disable();
    }
}
