using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round : MonoBehaviour
{
    [SerializeField] Vector2 startingPoint;
    [SerializeField] float startingRotation;

    RoundsManager manager;
    PlayerCharacter player;

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
        player?.SpawnAt(startingPoint, startingRotation);

        // reposiciona e respawna objetos
        // resetta 
    }

    public void RoundCleared()
    {
        if (manager)
            manager.NextRoundLogic();
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(startingPoint, .2f);
        Vector2 rotatedVector = RaposUtil.RotateVector(Vector2.down * .3f, startingRotation);
        Gizmos.DrawWireCube(startingPoint + rotatedVector, Vector3.one * .25f);
        //Gizmos.DrawLine(startingPoint, startingPoint + rotatedVector);
    }
}
