using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractablePortal : Interactable
{
    [SerializeField] BuildIndex targetIndex;
    [SerializeField] RoundSessionData data;
    //[SerializeField] SpriteSwapper ballonSpriteSwapper;

    public override void Interaction (PlayerInteractor interactor) 
    {
        base.Interaction(interactor);

        if (data)
        {
            //DialogueSystem dialogSystem = DialogueSystem.Instance;
            //dialogSystem?.SetDialogue(this, data.npcName, data.dialog);

            //if (interactor)
            //{
            //    PlatformerCharacter platformerCharacter = interactor.GetComponent<PlatformerCharacter>();
            //    platformerCharacter?.KillInputs();
            //    platformerCharacter?.LookAtTarget(transform);
            //}

            data.outroScene = (BuildIndex) SceneManager.GetActiveScene().buildIndex;
            data.OnSessionCompleted += () => 
            {
                Debug.Log("Session Done! ");
                SpawnManager.index = data.spawnIndex;
            };
            RoundsManager.SessionData = data;
            
            SceneTransition.LoadScene( (int) targetIndex /*(int) BuildIndex.TestDangerStage*/ );
        }
    }

    protected override void HighlightState (bool value) { }

    public override void IconState (bool value) { }
}
