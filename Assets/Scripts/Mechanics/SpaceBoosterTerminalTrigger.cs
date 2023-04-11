using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBoosterTerminalTrigger : SpaceBooster
{
    public SpaceBoosterTerminalEvent _event;

    protected override void PlayerLaunch(SpaceJumper spaceJumper)
    {
        base.PlayerLaunch(spaceJumper);
        if(_event)
            _event.Activate(null, null);
    }

    protected override void CollectableLaunch(CollectableThrowable collectable, Rigidbody2D rb)
    {
        base.CollectableLaunch(collectable, rb);
        if(_event)
            _event.Activate(null, null);
    }
    
}
