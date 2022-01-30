using System;
using UnityEngine;

public class LocalGameplayState : MonoBehaviour
{
    public GameplayState state;
    public AK.Wwise.Switch explorationState;
    public AK.Wwise.Switch dangerState;

    public SoundtrackManager soundtrackManager;
    
    //TODO limpar essa nojeira
    private void Update()
    {
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
