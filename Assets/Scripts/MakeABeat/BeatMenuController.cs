using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MakeABeat
{
    public enum MakeABeatFocus 
    {
        Tapes,
        Box,
        Menu
    }

    public class BeatMenuController : MonoBehaviour
    {
        [SerializeField] float navigationCooldown;

        [Header("References")]
        [SerializeField] BeatMaster beatMaster;
        [SerializeField] BeatTrackNavigation trackNavigation;
        [SerializeField] TapeBox tapeBox;

        float t;

        PlayerInputActions inputActions;
        InputAction navigationInput;

        public static MakeABeatFocus Focus;

        private void OnEnable() 
        {
            inputActions = new PlayerInputActions();
            inputActions.Enable();
            
            navigationInput = inputActions.UI.Navigation;
            navigationInput.Enable();

            inputActions.UI.Confirm.performed += (ctx) => 
            {
                ConfirmLogic();
            };
            inputActions.UI.Confirm.Enable();

            inputActions.UI.Cancel.performed += (ctx) =>
            {
                CancelLogic();
            };
            inputActions.UI.Cancel.Enable();

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
            NavigationLogic();
        }

        private void NavigationLogic()
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

            switch (Focus)
            {
                case MakeABeatFocus.Tapes:
                    trackNavigation.MoveCursor(direction.normalized);          
                    break;


                case MakeABeatFocus.Box:
                    tapeBox.MoveCursor (direction.x > 0 ? 1 : -1);
                    break;

                default:
                    return;
            }

            t = navigationCooldown;
        }
        
        private void ConfirmLogic()
        {
            switch (Focus)
            {
                case MakeABeatFocus.Tapes:
                    BeatTrack track = trackNavigation.GetSelectedTrack();
                    if (track == null)
                        return;

                    track.CloseTheLid(false);
                    trackNavigation.SetArrowsVisibility(false);
                    tapeBox.Show(track, true);
                    break;

                case MakeABeatFocus.Box:
                    tapeBox.OnSelectInput();
                    break;
            }
        }

        private void CancelLogic()
        {
            switch (Focus)
            {
                case MakeABeatFocus.Tapes:
                    trackNavigation.OnCancelInput(tapeBox);
                    break;

                case MakeABeatFocus.Box:
                    trackNavigation.SetArrowsVisibility(true);
                    BeatTrack track = trackNavigation.GetSelectedTrack();
                    if (track)
                    {
                        track.SetBoxPreviewState(null);
                        track.RestoreVisualState();
                    }

                    tapeBox.Show(null, false);
                    break;
            }
        }

        private void OnDisable() 
        {
            inputActions.Disable();
            
            navigationInput.Disable();
            inputActions.UI.Confirm.Disable();
            inputActions.UI.Cancel.Disable();
            inputActions.UI.Start.Disable();
        }
    }
}