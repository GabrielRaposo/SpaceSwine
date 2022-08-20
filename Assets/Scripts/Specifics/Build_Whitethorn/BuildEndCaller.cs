using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhitethornBuild
{
    public class BuildEndCaller : MonoBehaviour
    {
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

            buildOutroCardDisplay.CallScreen();
        }
    }
}