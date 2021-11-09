using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableNPC))]
public class DialogueJumpToIndex : MonoBehaviour
{
    [SerializeField] int currentIndex;
    [SerializeField] int jumpToTndex;

    void Start()
    {
        InteractableNPC npc = GetComponent<InteractableNPC>();
        npc.OnPreviousIndexReached += (i, data) => 
        {
            
        };
    }
}
