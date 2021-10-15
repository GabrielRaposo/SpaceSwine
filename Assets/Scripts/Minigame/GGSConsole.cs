﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Jumper;

namespace Minigame
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GGSConsole : MonoBehaviour
    {
        [SerializeField] float duration;
        [SerializeField] RenderTexture minigameRenderTexture;
        [SerializeField] GGSSplashScreen splashScreen;

        CanvasGroup canvasGroup;
        AsyncOperation asyncMinigameLoad;
        GGSMinigame pluggedCard;

        public static bool TurnedOn { get; private set; }
        public static GGSConsole Instance;

        private void Awake() 
        {
            Instance = this;

            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }

        public void ToggleConsoleState()
        {
            if (TurnedOn)
                TurnConsoleOff();
            else
                TurnConsoleOn();
        }

        public void TurnConsoleOn()
        {
            TurnedOn = true;
            canvasGroup.DOFade(1, duration);
            splashScreen.Call
            (
                () => SetupMinigame (GGSMinigame.Jumper)
            );
        }

        public void TurnConsoleOff()
        {
            TurnedOn = false;
            canvasGroup.DOFade(0, duration);
            UnloadMinigame();
        }

        private void SetupMinigame (GGSMinigame ggsMinigame)
        {
            pluggedCard = ggsMinigame;

            BuildIndex buildIndex = GetCardIndex();
            StartCoroutine( AsyncLoadRoutine( (int) buildIndex) );
        }

        private BuildIndex GetCardIndex()
        {
            switch (pluggedCard)
            {
                default:
                case GGSMinigame.Jumper:
                    return BuildIndex.MinigameJumper;
            }
        }

        private IEnumerator AsyncLoadRoutine(int index)
        {
            asyncMinigameLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
            while (!asyncMinigameLoad.isDone)
                yield return new WaitForEndOfFrame();

            yield return new WaitForEndOfFrame();

            SetupComponents();
        }

        private void SetupComponents()
        {
            MJ_CameraController cameraController = MJ_CameraController.Instance;
            if (cameraController)
                cameraController.SetRenderTexture(minigameRenderTexture);

            MJ_GameManager gameManager = MJ_GameManager.Instance;
            if (gameManager)
                gameManager.ConnectToConsole(this);
        }

        private void UnloadMinigame()
        {
            if (asyncMinigameLoad == null)
                return;

            BuildIndex buildIndex = GetCardIndex();
            SceneManager.UnloadSceneAsync((int) buildIndex);
        }

        public void ReloadMinigame()
        {
            UnloadMinigame();

            BuildIndex buildIndex = GetCardIndex();
            StartCoroutine(AsyncLoadRoutine((int) buildIndex) );
        }
    }
}

public enum GGSMinigame
{
    None,
    Jumper
}