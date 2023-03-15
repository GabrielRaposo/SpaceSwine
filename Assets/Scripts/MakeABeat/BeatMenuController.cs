using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MakeABeat
{
    public class BeatMenuController : MonoBehaviour
    {
        [SerializeField] float navigationCooldown;

        [Header("References")]
        [SerializeField] BeatMaster beatMaster;
        [SerializeField] BeatTrackNavigation trackNavigation;

        float t;

        PlayerInputActions inputActions;
        InputAction navigationInput;

        private void OnEnable() 
        {
            inputActions = new PlayerInputActions();
            inputActions.Enable();
            
            navigationInput = inputActions.UI.Navigation;
            navigationInput.Enable();

            //inputActions.UI.Navigation.performed += (ctx) =>
            //{
            //    Vector2 input = ctx.ReadValue<Vector2>();

            //    //int direction = 0;

            //    //if (input.x > 0 || input.y < 0)
            //    //    direction = 1;
            //    //else 
            //    //    direction = -1;

            //    if (trackNavigation)
            //        trackNavigation.MoveCursor(input.normalized);
            //};

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

        private void Update() 
        {
            Vector2 direction = navigationInput.ReadValue<Vector2>();
            if (direction == Vector2.zero) 
            {
                t = 0;
                return;
            }

            if (t > 0)
            {
                t -= Time.deltaTime;
                return;
            }

            if (!trackNavigation)
                return;

            trackNavigation.MoveCursor(direction.normalized);          
            t = navigationCooldown;
        }

        private void OnDisable() 
        {
            inputActions.Disable();
            
            navigationInput.Disable();
            inputActions.UI.Start.Disable();
        }
    }
}