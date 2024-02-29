using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAchievementsVerifier : MonoBehaviour
{
    public List<StoryEventScriptableObject> World1Events;
    public List<StoryEventScriptableObject> World2Events;
    public List<StoryEventScriptableObject> World3Events;

    private void Start()
    {
        CheckAchievements();
    }

    public void CheckAchievements()
    {
        bool allComplete = true;

        foreach (StoryEventScriptableObject storyEventScriptableObject in World1Events)
        {
            if (!StoryEventsManager.IsComplete(storyEventScriptableObject))
            {
                allComplete = false;
                break;
            }
        }
        
        if(allComplete)
            AchievementsManager.SetAchievementState(AchievementEnum.AsimovsLaws, true);

        allComplete = true;

        foreach (StoryEventScriptableObject storyEventScriptableObject in World2Events)
        {
            if (!StoryEventsManager.IsComplete(storyEventScriptableObject))
            {
                allComplete = false;
                break;
            }
        }
        
        if(allComplete)
            AchievementsManager.SetAchievementState(AchievementEnum.Cryptogeologist, true);

        allComplete = true;

        foreach (StoryEventScriptableObject storyEventScriptableObject in World3Events)
        {
            if (!StoryEventsManager.IsComplete(storyEventScriptableObject))
            {
                allComplete = false;
                break;
            }
        }
        
        if(allComplete)
            AchievementsManager.SetAchievementState(AchievementEnum.FruitsOfLabor, true);

    }
    
}
