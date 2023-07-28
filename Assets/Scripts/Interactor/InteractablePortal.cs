using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DevLocker.Utils;

public class InteractablePortal : Interactable
{
    [SerializeField] SceneReference targetScene;
    [SerializeField] RoundSessionData data;
    //[SerializeField] SpriteSwapper ballonSpriteSwapper;
    [SerializeField] DoorAnimation doorAnimation;
    [SerializeField] GameObject inputHelper;

    private void Start()
    {
        if (inputHelper) 
            inputHelper.SetActive(false);
    }

    public override void Interaction (PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        if (data)
        {
            if (inputHelper)
                inputHelper.SetActive(false);

            //if (interactor)
            //{
            //    PlatformerCharacter platformerCharacter = interactor.GetComponent<PlatformerCharacter>();
            //    platformerCharacter?.KillInputs();
            //    platformerCharacter?.LookAtTarget(transform);
            //}

            if (!doorAnimation)
                doorAnimation = GetComponentInChildren<DoorAnimation>();

            if (interactor && doorAnimation)
            {
                doorAnimation.SetupAnimationDangerZone
                (
                    doorAnimation.GetComponent<Door>(), 
                    interactor.gameObject,
                    LoadSceneAction
                );
                return;
            }

            LoadSceneAction();
        }
    }

    // -- Passado para Static para que possa ser usado na cena de navegação também
    public static void PreCallSetups (string exitScenePath, RoundSessionData data)
    {
        PagerInteractionManager.ExitScenePath = exitScenePath;
        RoundsManager.OnSessionCompletedAction = () =>
        {
            Debug.Log("Session Done! ");
            if (SaveManager.Initiated)
                SaveManager.SetSpawnIndex (data.OutroSpawnIndex);
        };
        RoundsManager.SessionData = data;

        if (SaveManager.Initiated) 
        {
            SaveManager.SetSpawnPath (exitScenePath);
            SaveManager.SetSpawnIndex (data.AbandonSpawnIndex);
        }
    }

    private void LoadSceneAction()
    {
        if (targetScene == null)
            return;

        PreCallSetups (GameManager.CurrentScene, data);

        SceneTransition.LoadScene( targetScene.ScenePath, SceneTransition.TransitionType.SafetyToDanger );
    }


    protected override void HighlightState (bool value) 
    {
        if (inputHelper)
            inputHelper.SetActive(value && interactable);
    }

    public override void IconState (bool value) { }

    public override void SetInteraction (bool value) 
    {
        base.SetInteraction(value);

        Collider2D collider2D = GetComponent<Collider2D>();
        if (!collider2D)
            return;

        collider2D.enabled = value;
    }
}
