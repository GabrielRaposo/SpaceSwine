using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class ShipShuttleSystem : MonoBehaviour
{
    const float UPPER_ANCHOR_Y =  .6f; //-1.46
    const float LOWER_ANCHOR_Y = -.5f;

    [SerializeField] bool alwaysStartPlayerOnShuttle;

    [Space(5)]

    [SerializeField] GameObject playerObject;
    [SerializeField] float duration;
    [SerializeField] ShipScreensOverlay screensOverlay;
    
    [Header("Audio")]
    [SerializeField] AK.Wwise.Event reachingAKEvent;
    [SerializeField] AK.Wwise.Event leavingAKEvent;

    PlayerCharacter playerCharacter;
    PlayerInput playerInput;
    PlatformerCharacter platformerCharacter;

    Sequence sequence;

    public UnityAction AfterStartAction = null;

    public static ShipShuttleSystem Instance;

    private void Awake()
    {
        Instance = this;

        if (!playerObject)
        {
            enabled = false;
            return;
        }

        playerCharacter = playerObject.GetComponent<PlayerCharacter>();
        playerInput = playerObject.GetComponent<PlayerInput>();
        platformerCharacter = playerObject.GetComponent<PlatformerCharacter>();

        if (!playerCharacter || !playerInput || !platformerCharacter)
        {
            enabled = false;
            return;
        }

        PlayerTransitionState.EnterState = PlayerTransitionState.State.None;
    }

    private void Start() 
    {
        if (!alwaysStartPlayerOnShuttle)
            return;

        StartOnShuttle();        
    }

    private void StartOnShuttle()
    {
        playerCharacter.SetPhysicsBody(false);

        // É bom que o componente do PlayerInput esteja desligado nas cenas em que tem esse script
        playerCharacter.enabled = false;
        playerInput.enabled = false;
        platformerCharacter.StandStillState();

        playerObject.transform.position 
            = transform.position + (Vector3.up * LOWER_ANCHOR_Y); 
        Vector2 targetPosition 
            = transform.position + (Vector3.up * UPPER_ANCHOR_Y); 

        RaposUtil.WaitSeconds(this, duration: .55f, () => 
        {
            if (reachingAKEvent != null && playerObject.activeSelf)
                reachingAKEvent.Post(gameObject);
        });

        sequence = DOTween.Sequence();
        sequence.Append( playerObject.transform.DOMoveY( targetPosition.y, duration ).SetEase(Ease.Linear) );
        sequence.OnComplete
        (
            () => 
            {   
                if (screensOverlay)
                    screensOverlay.TurnOn();

                playerCharacter.SetPhysicsBody(true);
                playerCharacter.enabled = true;
                platformerCharacter.enabled = true;

                if (AfterStartAction == null) 
                {
                    RestorePlayerControls();
                    return;
                }

                AfterStartAction.Invoke();
            }
        );
    }

    public void RestorePlayerControls()
    {
        playerInput.enabled = true;
    }

    public void ExitOnShuttle()
    {
        PlayerTransitionState.EnterState = PlayerTransitionState.State.Teleport;
        playerCharacter.SetPhysicsBody(false);

        // É bom que o componente do PlayerInput esteja desligado nas cenas em que tem esse script
        playerCharacter.enabled = false;
        playerInput.enabled = false;
        platformerCharacter.StandStillState();

        playerObject.transform.position 
            = transform.position + (Vector3.up * UPPER_ANCHOR_Y); 
        Vector2 targetPosition 
            = transform.position + (Vector3.up * LOWER_ANCHOR_Y); 

        if (leavingAKEvent != null)
            leavingAKEvent.Post(gameObject);

        sequence = DOTween.Sequence();
        sequence.Append( playerObject.transform.DOMoveY( targetPosition.y, duration ).SetEase(Ease.Linear) );
        sequence.OnComplete
        (
            () => 
            {   
                // Troca pela última fase em que você estava antes de entrar na Nave ou região
                int targetIndex = NavigationConsole.ShipTeleportSceneIndex;
                SceneTransition.LoadScene( targetIndex, SceneTransition.TransitionType.BlackFade );
            }
        );
    }
}
