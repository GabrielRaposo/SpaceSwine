using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NavigationWorldManager : MonoBehaviour
{
    [SerializeField] private GameObject[] worlds;
    [SerializeField] private NavigationWorldTransition transition;

    public static int CurrentWorld = 1;
    public static NavigationWorldManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        Instance = this;
        
        //to-do: pega CurrentWorld do Save

        UpdateWorlds();
    }

    private void UpdateWorlds()
    {
        if (worlds == null)
            return;

        for (int i = 0; i < worlds.Length; i++)
            worlds[i].SetActive (i == CurrentWorld - 1);
    }

    public void ChangeWorld (int valueOffset)
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
                CurrentWorld += valueOffset;
                UpdateWorlds ();
            },
            afterAction: () =>
            {
                // TO-DO: restore controls 
            }
        );
    }
}
