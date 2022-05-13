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
        if (player)
        {
            //player.gameObject.SetActive(false);
                
            player.SpawnAt(startingPoint, startingRotation);
            player.transform.SetParent(transform);
            player.gameObject.SetActive(true);
        }

        // reposiciona e respawna objetos
        // resetta
        //OnReset?.Invoke();
        StartCoroutine(OnResetRoutine());
    }

    public IEnumerator OnResetRoutine()
    {
        yield return new WaitForEndOfFrame();
        OnReset?.Invoke();
        RaposUtil.ClearLog();
    }

    public void RoundCleared()
    {
        Debug.Log("RoundCleared()");
        
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
