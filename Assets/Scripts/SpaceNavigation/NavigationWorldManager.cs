using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NavigationWorldManager : MonoBehaviour
{
    [SerializeField] private GameObject[] worlds;
    [SerializeField] private NavigationWorldTransition transition;

    [Header("Debug")]
    [SerializeField] InputAction testAction;

    public static int CurrentWorld = 1;

    private void Awake()
    {
        //to-do: pega CurrentWorld do Save

        UpdateWorlds();
    }

    private void OnEnable()
    {
        testAction.Enable();
        testAction.performed += (ctx) => 
        {
            ChangeWorld (CurrentWorld+1);
        };
    }

    private void OnDisable()
    {
        testAction.Disable();
    }

    private void UpdateWorlds()
    {
        if (worlds == null)
            return;

        for (int i = 0; i < worlds.Length; i++)
            worlds[i].SetActive (i == CurrentWorld - 1);
    }

    public void ChangeWorld (int targetWorld)
    {
        if (transition == null)
        {
            return;
        }

        // TO-DO: disable controls 

        transition.CallTransition
        ( 
            midAction: () => 
            {
                CurrentWorld = targetWorld;
                UpdateWorlds ();
            },
            afterAction: () =>
            {
                // TO-DO: restore controls 
            }
        );
    }
}
