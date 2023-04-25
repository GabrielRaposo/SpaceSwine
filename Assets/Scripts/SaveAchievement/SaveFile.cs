﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveFile
{
    public int version = 1;

    public List<StoryEventData> storyEventsStates;

    //public List<MinigameHiscore> JumperHighscores;

    public List<AchievementLog> achievementLog;
    private float playtime;

    public SaveFile ()
    {
        storyEventsStates = new List<StoryEventData>();

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

[System.Serializable]
public class ItemIndexer
{
    public ItemIndexer (float x, float y, float id)
    {
        this.x = x;
        this.y = y;
        this.id = id;
    }

    public override string ToString()
    {
        return $"x: {x}, y: {y}, id: {id}";
    }

    public bool CompareTo(ItemIndexer item)
    {
        return (x == item.x) && (y == item.y) && (id == item.id);
    }

    [SerializeField] public float x;
    [SerializeField] public float y;
    [SerializeField] public float id;
}

[System.Serializable]
public class StoryEventData
{
    public StoryEventData(bool state, string name)
    {
        this.state = state;
        this.name = name;
    }

    [SerializeField] public bool state;
    [SerializeField] public string name;
}