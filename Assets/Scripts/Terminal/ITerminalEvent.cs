using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITerminalEvent
{
    void Activate (InteractableTerminal terminal, PlayerInteractor interactor);
}
