using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiterminalMoveAction : TerminalMoveAction
{
    public int terminalCount;
    int activatedCount;

    public override void Activate(InteractableTerminal terminal, PlayerInteractor player)
    {
        activatedCount++;
        if(activatedCount<terminalCount)
            return;        
        base.Activate(terminal, player);
    }

    protected override void OnReset()
    {
        activatedCount = 0;
        base.OnReset();
    }
}
