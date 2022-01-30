using System;
using UnityEngine;

public class LocalGameplayState : MonoBehaviour
{
    public GameplayState state;
    public AK.Wwise.Switch explorationState;
    public AK.Wwise.Switch dangerState;
    
    private void Start()
    {
        SoundtrackManager soundtrackManager = SoundtrackManager.Instance; 
        if (!soundtrackManager)
            return;

        switch (state)
        {
            case GameplayState.Exploration:
                explorationState.SetValue(soundtrackManager.gameObject);
                break;
            case GameplayState.Danger:
                dangerState.SetValue(soundtrackManager.gameObject);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
