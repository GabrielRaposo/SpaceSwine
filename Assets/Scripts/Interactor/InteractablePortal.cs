using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractablePortal : Interactable
{
    [SerializeField] BuildIndex targetIndex;
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

    private void LoadSceneAction()
    {
        data.outroScene = (BuildIndex) SceneManager.GetActiveScene().buildIndex;
        data.OnSessionCompleted += () => 
        {
            //Debug.Log("Session Done! ");
            Debug.Log("LoadSceneAction");
            SpawnManager.Index = data.OutroSpawnIndex;
        };
        RoundsManager.SessionData = data;
            
        SceneTransition.LoadScene( (int) targetIndex, SceneTransition.TransitionType.SafetyToDanger );
    }

    protected override void HighlightState (bool value) 
    {
        if (inputHelper)
            inputHelper.SetActive(value && interactable);
    }

    public override void IconState (bool value) { }
}
