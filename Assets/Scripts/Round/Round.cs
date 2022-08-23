using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Round : MonoBehaviour
{
    [SerializeField] Vector2 startingPoint;
    [SerializeField] float startingRotation;

    RoundsManager manager;
    PlayerCharacter player;

    public UnityAction OnReset;
    public UnityAction OnPortalReached;
    public UnityAction OnPassRound;

    public void SetReferences (RoundsManager manager, PlayerCharacter player)
    {
        this.manager = manager;
        this.player = player;
    }

    public void SetActivation (bool value)
    {
        gameObject.SetActive(value);
        if (value)
            SetupRound();
    }

    public void SetupRound()
    {
        StartCoroutine(OnResetRoutine());

        if (player)
        {
            player.gameObject.SetActive(false);
                
            player.SpawnAt(startingPoint, startingRotation);
            //player.transform.SetParent(transform);
            //player.gameObject.SetActive(true);

            StartCoroutine 
            ( 
                WaitForSignal 
                (
                    () => 
                    {
                        player.transform.SetParent(transform);
                        player.gameObject.SetActive(true);

                        PlayerTransitionState playerTransitionState = player.GetComponent<PlayerTransitionState>();
                        if (playerTransitionState)
                        {
                            PlayerTransitionState.EnterState = PlayerTransitionState.State.OutOfPortal;
                            playerTransitionState.CallOnStartSpawn();
                        }
                    }
                ) 
            );
        }

        //OnReset?.Invoke();
    }

    private IEnumerator WaitForSignal (UnityAction action)
    {
        yield return new WaitWhile( () => RoundsManager.BlockSpawn );
        //Debug.Log("Call");
        action.Invoke();
    } 

    public IEnumerator OnResetRoutine()
    {
        yield return new WaitForEndOfFrame();
        OnReset?.Invoke();
        RaposUtil.ClearLog();
    }

    public void RoundCleared()
    {
        //Debug.Log("RoundCleared()");
        
        OnPassRound?.Invoke();
        
        if (manager)
            manager.NextRoundLogic();
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(startingPoint, .2f);
        Vector2 rotatedVector = RaposUtil.RotateVector(Vector2.down * .3f, startingRotation);
        Gizmos.DrawWireCube(startingPoint + rotatedVector, Vector3.one * .25f);
    }
}
