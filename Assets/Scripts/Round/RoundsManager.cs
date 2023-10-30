using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DevLocker.Utils;

public class RoundsManager : MonoBehaviour
{
    [SerializeField] InputAction previousInputAction;
    [SerializeField] InputAction resetInputAction;
    [SerializeField] InputAction nextInputAction;

    [Header ("SessionData")]
    [SerializeField] int lastIndex;
    [SerializeField] SceneReference exitScene;
    [SerializeField] int localAbandonSpawnIndex;
    [SerializeField] int localOutroSpawnIndex;
    [SerializeField] StoryEventScriptableObject completionStoryEvent;

    public static int AbandonSpawnIndex;
    public static int OutroSpawnIndex;

    int currentIndex;
    List<Round> rounds;
    PlayerCharacter player;

    public static bool BlockSpawn;
    public static UnityAction OnSessionCompletedAction;

    public static RoundsManager Instance;

    private void Awake() 
    {
        Instance = this;    

        AbandonSpawnIndex = localAbandonSpawnIndex;
        OutroSpawnIndex = localOutroSpawnIndex;
    }

    private void Start() 
    {
        // -- Define câmera de fases de perigo
        CameraFocusController cameraFocusController = CameraFocusController.Instance;
        if (cameraFocusController)
            cameraFocusController.SetStaticFocus();
        
        // -- Encontra a instância do player
        player = PlayerCharacter.Instance;
        if (!player)
        {
            Debug.LogError("No player found.");
            return;
        }

        // -- Cria lista de rounds
        rounds = new List<Round>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Round r = transform.GetChild(i).GetComponent<Round>();
            if (r) 
            {
                r.SetReferences(this, player);
                rounds.Add(r);
            }
        }
        if (rounds.Count < 1)
        {
            Debug.LogError("No round found.");
            return;
        }

        // -- Encontra o Index inicial
        currentIndex = 0;

        ActivateCurrentIndex();
        
        // -- Chamada de reset de cena na morte do player
        PlayerCharacter playerCharacter = player.GetComponent<PlayerCharacter>();
        if (playerCharacter)
            playerCharacter.SetDeathEvent( () => RoundTransition.Call(ActivateCurrentIndex) );

        player.gameObject.SetActive(false);

        // -- Setup de inputs de teste
        //#if UNITY_EDITOR
        previousInputAction.performed += ctx => 
        {
            if (GameManager.BlockCharacterInput || GameManager.OnTransition || PauseSystem.OnPause)
                return;

            PreviousRoundLogic();
        };
        previousInputAction.Enable();

        nextInputAction.performed += ctx => 
        {
            if (GameManager.BlockCharacterInput || GameManager.OnTransition || PauseSystem.OnPause)
                return;

            if (currentIndex < rounds.Count)
                rounds[currentIndex].RoundCleared();
        };
        nextInputAction.Enable();
        //#endif
    }

    public void PreviousRoundLogic()
    {
        int first = 0;

        if (currentIndex > first)
        {
            currentIndex--;
            RoundTransition.Call(ActivateCurrentIndex);
        }
    }

    private void ActivateCurrentIndex()
    {
        currentIndex %= rounds.Count;
        for (int i = 0; i < rounds.Count; i++)
        {
            //int localIndex = i;
            //rounds[i].SetActivation(i == currentIndex);
            if (i != currentIndex)
                rounds[i].SetActivation(false);
            //else
            //    //StartCoroutine(RaposUtil.Wait(1, () => rounds[localIndex].SetActivation(true)));
            //    rounds[localIndex].SetActivation(true);
        }

        rounds[currentIndex].SetActivation(true);
    }

    public void NextRoundLogic()
    {
        currentIndex++;

        int lastRound = rounds.Count;
        if (lastIndex > 0)
        {
            lastRound = lastIndex;
        }

        if (currentIndex - 1 < lastRound)
        {
            RoundTransition.Call(ActivateCurrentIndex);
        } 
        else
        {
            if (OnSessionCompletedAction != null)
                OnSessionCompletedAction();

            if (completionStoryEvent != null && SaveManager.Initiated)
                StoryEventsManager.ChangeProgress(completionStoryEvent, 99);

            SceneTransition.LoadScene( exitScene.ScenePath, SceneTransition.TransitionType.DangerToSafety );
            SaveManager.SetSpawnPath( exitScene.ScenePath );
        }
    }

    public void CallReset()
    {
        RoundTransition.Call(ActivateCurrentIndex);
    }

    private void OnDisable()
    {
        //#if UNITY_EDITOR
        previousInputAction.Disable();
        nextInputAction.Disable();
        //#endif
    }
}
