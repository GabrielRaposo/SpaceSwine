using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhitethornBuild
{
    public class BuildEndCaller : MonoBehaviour
    {
        [SerializeField] ShipDialogueBox dialogueBox;

        BuildOutroCardDisplay buildOutroCardDisplay;

        private void Awake () 
        {
            buildOutroCardDisplay = GetComponentInChildren<BuildOutroCardDisplay>();
        }

        public void SetEndBuildOutro (PlayerInteractor interactor)
        {
            PlayerCharacter player = interactor.GetComponent<PlayerCharacter>();

            if (!player)
                return;

            GameManager.BlockCharacterInput = true;
            player.DisableAllInteractions();

            CallDialogue();
        }

        private void CallDialogue()
        {
            if (!dialogueBox)
            {
                buildOutroCardDisplay.CallScreen();
                return;
            }


        }
    }
}