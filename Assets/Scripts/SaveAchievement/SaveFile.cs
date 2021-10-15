using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveFile
{
    public int version = 2;
    public List<AchievementLog> achievementLog;
    private float playtime;

    public SaveFile ()
    {
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
    
    public float Playtime
    {
        get => playtime;
        set => playtime = value;
    }
    
    public float GetPlaytime()
    {
        return Playtime;
    }

    public void AddPlaytime(float sessionTime)
    {
        Playtime += sessionTime;
    }
}



