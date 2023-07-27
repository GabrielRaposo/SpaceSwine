using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DevLocker.Utils;

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
        [SerializeField] AK.Wwise.RTPC sfxAKParam;
        [SerializeField] SceneReference exitScene;
        [SerializeField] SceneReference titleScene;

        [Header("Temp")]
        [SerializeField] InputAction toggleSFXsAction;

        float t;
        bool mutedSFX;

        PlayerInputActions inputActions;
        InputAction navigationInput;

        public static MakeABeatFocus Focus;
        public static bool ExitToTitle;

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

            inputActions.UI.Other.performed += (ctx) => 
            {
                OtherLogic();
            };
            inputActions.UI.Other.Enable();

            inputActions.UI.Start.performed += (ctx) => 
            {
                if (Focus != MakeABeatFocus.Tapes)
                    return;

                // TO-DO: Abre menu lateral e dá fade out 40% na tela
                ExitScreen();
            };
            inputActions.UI.Start.Enable();

            toggleSFXsAction.performed += (ctx) => 
            {
                if (sfxAKParam == null)
                    return;

                mutedSFX = !mutedSFX;
                sfxAKParam.SetGlobalValue(mutedSFX ? 0 : 100);
            };
            toggleSFXsAction.Enable();
        }

        private void ExitScreen()
        {
            if (beatMaster)
                beatMaster.StopCycle();

            ResumePlaylistOnStart.Trigger = true;
            ShipInitializerSystem.MakeABeatSpawnTrigger = true;
            Focus = MakeABeatFocus.Tapes;

            GameManager.GoToScene 
            (
                ExitToTitle ? titleScene.ScenePath : exitScene.ScenePath, 
                saveScenePath: !ExitToTitle
            );
            
            enabled = false;
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
                    int input = 1;
                    if (direction.x != 0) 
                        input = direction.x > 0 ? 1 : -1;
                    if (direction.y != 0)
                        input = direction.y < 0 ? 1 : -1;

                    tapeBox.MoveCursor (input);
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
                    trackNavigation.OnConfirmInput(tapeBox);
                    break;

                case MakeABeatFocus.Box:
                    tapeBox.OnSelectInput();
                    trackNavigation.SetArrowsVisibility(true);
                    Focus = MakeABeatFocus.Tapes;
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
                    tapeBox.Show(null, false);
                    Focus = MakeABeatFocus.Tapes;
                    break;
            }
        }

        private void OtherLogic()
        {
            switch (Focus)
            {
                case MakeABeatFocus.Tapes:
                    trackNavigation.OnOtherInput();
                    break;
            }
        }

        private void OnDisable() 
        {
            inputActions.Disable();
            
            navigationInput.Disable();
            inputActions.UI.Confirm.Disable();
            inputActions.UI.Cancel.Disable();
            inputActions.UI.Other.Disable();
            inputActions.UI.Start.Disable();

            toggleSFXsAction.Disable();
        }
    }
}