using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallInteractableNPCOnContact : MonoBehaviour
{
    [SerializeField] float waitTime;
    [SerializeField] InteractableNPCSimplified NPC;

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (NPC == null)
            return;

        PlayerInteractor player = collision.GetComponent<PlayerInteractor>();
        if (!player)
            return;
        
        NPC.StartCoroutine(BlockPlayerAndCallDialogue(player));
        gameObject.SetActive(false);
    }

    private IEnumerator BlockPlayerAndCallDialogue(PlayerInteractor player)
    {
        NPC.Interaction(player);
        //NPC.Interaction(player);
        yield break;
    }
}
