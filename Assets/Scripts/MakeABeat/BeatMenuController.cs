using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MakeABeat
{
    public class BeatMenuController : MonoBehaviour
    {
        [SerializeField] BeatMaster beatMaster;
        [SerializeField] BeatTrackNavigation trackNavigation;

        PlayerInputActions inputActions;

        private void OnEnable() 
        {
            inputActions = new PlayerInputActions();
            inputActions.Enable();

            inputActions.UI.Navigation.performed += (ctx) =>
            {
                Vector2 input = ctx.ReadValue<Vector2>();

                int direction = 0;

                if (input.x > 0 || input.y < 0)
                    direction = 1;
                else 
                    direction = -1;

                if (trackNavigation)
                    trackNavigation.MoveCursor(direction);
            };

            inputActions.UI.Start.performed += (ctx) => 
            {
                // TO-DO: Abre menu lateral e dá fade out 40% na tela
                ExitScreen();
            };
            inputActions.UI.Start.Enable();
        }

        private void ExitScreen()
        {
            if (beatMaster)
                beatMaster.StopCycle();

            ShipInitializerSystem.MakeABeatSpawnTrigger = true;
            GameManager.GoToScene(BuildIndex.Ship);
            
            enabled = this;
        }

        private void OnDisable() 
        {
            inputActions.Disable();    
            inputActions.UI.Start.Disable();
        }
    }
}