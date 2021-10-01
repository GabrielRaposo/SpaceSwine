using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoundsManager : MonoBehaviour
{
    [SerializeField] InputAction previousInputAction;
    [SerializeField] InputAction resetInputAction;
    [SerializeField] InputAction nextInputAction;

    int currentIndex;
    List<Round> rounds;
    PlayerCharacter player;

    public static bool CustomIniciation = false;
    // public static RoundSessionData roundSessionData;

    private void Start() 
    {
        // Encontra a instância do player
        player = PlayerCharacter.Instance;
        if (!player)
        {
            Debug.LogError("No player found.");
            return;
        }

        // Cria lista de rounds
        rounds = new List<Round>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Round r = transform.GetChild(i).GetComponent<Round>();
            if (r) 
            {
                r.SetReferences(this, player);
                rounds.Add(r);
            }
        }
        if (rounds.Count < 1)
        {
            Debug.LogError("No round found.");
            return;
        }

        // Encontra o Index inicial
        if (CustomIniciation)
        {
            
        }
        else currentIndex = 0;

        ActivateCurrentIndex();

        previousInputAction.performed += ctx => 
        {
            PreviousRoundLogic();
        };
        previousInputAction.Enable();

        resetInputAction.performed += ctx => 
        {
            RoundTransition.Call(ActivateCurrentIndex);
        };
        resetInputAction.Enable();

        nextInputAction.performed += ctx => 
        {
            if (currentIndex < rounds.Count)
                rounds[currentIndex].RoundCleared();
        };
        nextInputAction.Enable();

        Health health = player.GetComponent<Health>();
        if (health)
            health.OnDeathEvent += () => RoundTransition.Call(ActivateCurrentIndex);
    }

    public void PreviousRoundLogic()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            RoundTransition.Call(ActivateCurrentIndex);
        }
    }

    private void ActivateCurrentIndex()
    {
        currentIndex %= rounds.Count;
        for (int i = 0; i < rounds.Count; i++)
            rounds[i].SetActivation( i == currentIndex );        
    }

    public void NextRoundLogic()
    {
        currentIndex++;
        if (currentIndex < rounds.Count)
        {
            // ActivateCurrentIndex();
            RoundTransition.Call(ActivateCurrentIndex);
        }
    }
}
