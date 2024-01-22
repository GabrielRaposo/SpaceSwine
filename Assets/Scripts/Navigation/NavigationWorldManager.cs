using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

public class NavigationWorldManager : MonoBehaviour
{
    [SerializeField] private string worldLabelID;
    [SerializeField] private TextMeshProUGUI display;
    [SerializeField] private UpdateNavigationMainCanvas mainCanvas;

    [Header("")]
    [SerializeField] private NavigationShipLandAnimation ship;
    [SerializeField] private NavigationWorldGroup[] worlds;
    [SerializeField] private NavigationWorldTransition transition;

    public static int CurrentWorld = 0;
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
        if (SaveManager.Initiated) 
            CurrentWorld = SaveManager.CurrentWorld;

        UpdateWorlds();
        UpdateShipColors();
    }

    private void UpdateDisplay()
    {
        if (!display)
            return;

        string text = LocalizationManager.GetUiText(worldLabelID, fallback: "World");
        display.text = text + " " + ( CurrentWorld + 1 );

        if (worlds == null && worlds.Length < 1)
            return;

        display.color = GetWorldGroup().SelectedColor;
    }

    private void UpdateWorlds()
    {
        if (worlds == null)
            return;

        for (int i = 0; i < worlds.Length; i++)
            worlds[i].SetActive (i == CurrentWorld);

        UpdateDisplay();
        UpdateMainCanvasColors();
    }

    private void UpdateMainCanvasColors()
    {
        if (mainCanvas == null)
            return;

        mainCanvas.UpdateColors( GetWorldGroup().SelectedColor, GetWorldGroup().UnselectedColor, GetWorldGroup().BackgroundColor );
    }

    private void UpdateShipColors()
    {
        if (!ship)
            return;

        ship.UpdateColors(GetWorldGroup().SelectedColor, GetWorldGroup().UnselectedColor, GetWorldGroup().BackgroundColor);
    }

    public NavigationWorldGroup GetWorldGroup()
    {
        if (CurrentWorld < 0) 
            return null;

        return worlds [ CurrentWorld % worlds.Length ];
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
                if (SaveManager.Initiated)
                {
                    SaveManager.CurrentWorld = CurrentWorld;
                    SaveManager.Save();
                }
                UpdateWorlds ();

                NavigationWorldGroup worldGroup = GetWorldGroup();
                ship.JumpToPosition (valueOffset > 0 ? worldGroup.SpawnPoint : worldGroup.ReturnPoint);
                ship.UpdateColors (worldGroup.SelectedColor, worldGroup.UnselectedColor, worldGroup.BackgroundColor);

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
