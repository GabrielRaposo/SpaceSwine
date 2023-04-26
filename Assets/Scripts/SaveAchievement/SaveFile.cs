using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveFile
{
    public int version = 1;

    //public List<StoryEventData> storyEventsStates;

    //public List<MinigameHiscore> JumperHighscores;

    public List<AchievementLog> achievementLog;

    private float playtime;

    public float Playtime
    {
        get => playtime;
        set => playtime = value;
    }

    public SaveFile ()
    {
        //storyEventsStates = new List<StoryEventData>();

        var achievementList = AchievementsManager.GetNewAchievementList();

        achievementLog = new List<AchievementLog>();

        foreach (Achievement achievement in achievementList)
        {
            var aLog = new AchievementLog(achievement);
            achievementLog.Add(aLog);
        }
    }

    private List<StageProgress> GetBaseLevelList()
    {
        //TEMP
        
        return new List<StageProgress>();
    }
  
    public string PrintStoredData()
    {
        string s = "Stored Data on Save: \n";

        return s;
    }
}