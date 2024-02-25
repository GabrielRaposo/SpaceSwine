using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRewarder : MonoBehaviour
{
    [SerializeField] int reward = 1;
    [SerializeField] InteractableNPC afterDialogueNPC;

    public void Call (PlayerInteractor playerInteractor)
    {
        ItemRewardBox rewardBox = ItemRewardBox.Instance;
        if (!rewardBox)
            return;

        if (afterDialogueNPC == null)
        {
            rewardBox.Call 
            ( 
                reward, 
                playerInteractor, 
                AfterRewardAction: null 
            );
            return;
        }

        rewardBox.Call
        (
            reward, 
            playerInteractor, 
            AfterRewardAction: () => afterDialogueNPC.Interaction(playerInteractor)
        );
    }
}
