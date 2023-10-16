using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class NavigationWorldManager : MonoBehaviour
{
    [SerializeField] private string worldLabelID;
    [SerializeField] private TextMeshProUGUI display;

    [Header("")]
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
    }

    private void Start()
    {
        UpdateWorlds();
    }

    private void UpdateDisplay()
    {
        if (!display)
            return;

        string text = LocalizationManager.GetUiText(worldLabelID, fallback: "World");
        display.text = text + " " + CurrentWorld;
    }

    private void UpdateWorlds()
    {
        if (worlds == null)
            return;

        for (int i = 0; i < worlds.Length; i++)
            worlds[i].SetActive (i == CurrentWorld - 1);

        UpdateDisplay();
    }

    public void ChangeWorld (int valueOffset, NavigationShipLandAnimation ship)
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
                ship.ClearScreenState();
            },
            afterAction: () =>
            {
                // TO-DO: get spawn point
                // TO-DO: call spawn animation

                ship.UnlockControls();
            }
        );
    }
}
