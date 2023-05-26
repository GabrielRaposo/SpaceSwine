using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBoosterTerminalEvent : MonoBehaviour, ITerminalEvent
{
    [SerializeField]private SpaceBooster _spaceBooster;
    public Vector2 positionToSet;

    public void Activate(InteractableTerminal terminal, PlayerInteractor interactor)
    {
        _spaceBooster.SetFirstLaunchDirection(positionToSet.x, positionToSet.y);
    }
}
