using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallInteractableNPCOnContact : StoryEventDependent
{
    [SerializeField] float waitTime;
    [SerializeField] StoryEventScriptableObject storyEvent;
    [SerializeField] InteractableNPCSimplified NPC;

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (NPC == null)
            return;

        PlayerInteractor player = collision.GetComponent<PlayerInteractor>();
        if (!player)
            return;
        
        CallDependentAction
        (
            () => 
            {
                NPC.StartCoroutine(BlockPlayerAndCallDialogue(player));
                gameObject.SetActive(false);
            }
        );

    }

    private IEnumerator BlockPlayerAndCallDialogue(PlayerInteractor player)
    {
        if (storyEvent != null && StoryEventsManager.IsComplete(storyEvent))
            yield break;

        GameManager.BlockCharacterInput = true;

        PlatformerCharacter platformerCharacter = player.GetComponent<PlatformerCharacter>();
        platformerCharacter?.KillInputs();

        yield return new WaitForSeconds(waitTime);

        NPC.Interaction(player);
        GameManager.BlockCharacterInput = false;
    }
}
