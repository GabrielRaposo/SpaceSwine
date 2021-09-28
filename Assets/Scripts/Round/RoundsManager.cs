using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundsManager : MonoBehaviour
{
    int currentIndex;
    List<Round> rounds;

    public static bool CustomIniciation = false;
    // public static RoundSessionData roundSessionData;

    private void Awake() 
    {
        // Cria lista de rounds
        rounds = new List<Round>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Round r = transform.GetChild(i).GetComponent<Round>();
            if (r) rounds.Add(r);
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
    }

    private void ActivateCurrentIndex()
    {
        currentIndex %= rounds.Count;
        for (int i = 0; i < rounds.Count; i++)
            rounds[i].SetActivation( i == currentIndex );        
    }



    void Start()
    {
        
    }
}
