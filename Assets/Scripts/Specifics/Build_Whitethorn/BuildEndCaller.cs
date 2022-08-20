using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace WhitethornBuild
{
    public class BuildEndCaller : MonoBehaviour
    {
        [SerializeField] List<string> dialogueIDs; 

        [Header("References")]
        [SerializeField] ShipDialogueBox dialogueBox;
        [SerializeField] CameraSizeController sizeController;

        int currentIndex;

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

            if (!dialogueBox)
                return;

            float t = 1.0f;
            DOVirtual.Float(from: 3f, to: 2f, t, (f) => CameraSizeController.Size = f );

            float duration = .5f + t;
            dialogueBox.SetShown(true);
            this.WaitSeconds(duration, () => 
            {
                currentIndex = 0;
                CallDialogue();
            });
        }

        // -- TO-DO: fazer uma função assim automatizada pra própria caixa da nave, onde você entra com um grupo de strings
        private void CallDialogue() 
        {
            if (!dialogueBox || dialogueIDs.Count < 1)
            {
                buildOutroCardDisplay.CallScreen();
                return;
            }

            // -- End Dialogue
            if (currentIndex > dialogueIDs.Count - 1)
            {
                float duration = .5f;
                dialogueBox.SetShown(false, duration);
                this.WaitSeconds(duration * 2f, () => buildOutroCardDisplay.CallScreen() );
                return;
            }

            (bool isValid, string text) data = LocalizationManager.GetShipText(dialogueIDs[currentIndex % dialogueIDs.Count]);
            if (!data.isValid)
            {
                buildOutroCardDisplay.CallScreen();
                return;
            }

            dialogueBox.Type (data.text, delay: .3f, instantText: false, 
                afterInputAction: 
                () => {
                    currentIndex++;
                    CallDialogue();
                }
            );
        }
    }
}