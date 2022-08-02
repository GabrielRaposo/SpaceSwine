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

    public override void Interaction (PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        if (data)
        {
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
                doorAnimation.SetupAnimation
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
            SpawnManager.index = data.spawnIndex;
        };
        RoundsManager.SessionData = data;
            
        SceneTransition.LoadScene( (int) targetIndex, SceneTransition.TransitionType.SafetyToDanger );
    }

    protected override void HighlightState (bool value) { }

    public override void IconState (bool value) { }
}
