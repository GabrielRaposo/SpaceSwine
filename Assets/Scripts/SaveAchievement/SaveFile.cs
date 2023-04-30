using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveFile
{
    public int version = 1;

    // -- Spawn Data
    public string spawnScenePath;
    public int spawnIndex;

    // -- Navigation Data
    public int currentWorld;
    public NavigationShipData navigationShipData;
    public string shipLandedLocationPath;

    // -- Events and UI Notifications
    public List<EventProgressData> eventProgressList;
    public List<UINotification> uiNotificationsList;

    // -- Minigame Scores
    //public List<MinigameHiscore> JumperHighscores;
    //public List<MinigameHiscore> ShooterHighscores;

    public List<AchievementLog> achievementLog;

    private float playtime;

    public float Playtime
    {
        get => playtime;
        set => playtime = value;
    }

    public SaveFile ()
    {
        PrepFile();

        // -- Achievement
        {
            var achievementList = AchievementsManager.GetNewAchievementList();

            achievementLog = new List<AchievementLog>();

            foreach (Achievement achievement in achievementList)
            {
                var aLog = new AchievementLog(achievement);
                achievementLog.Add(aLog);
            }
        }
    }

    // -- Classes geradas pela serialiazação podem não possuir a inicialização de novos componentes
    public void PrepFile()
    {
        if (navigationShipData == null)
            navigationShipData = new NavigationShipData();

        if (eventProgressList == null)
            eventProgressList = new List<EventProgressData>();

        if (uiNotificationsList == null)
            uiNotificationsList = new List<UINotification>();

        if (achievementLog == null)
            achievementLog = new List<AchievementLog>();
    }
  
    public string PrintStoredData()
    {
        string s = "Stored Data on Save: \n";

        if (eventProgressList != null)
        {   
            s += $"\tStoryEventsProgress.Count: { eventProgressList.Count } \n";

            for (int i = 0; i < eventProgressList.Count; i++)
            {
                EventProgressData progressData = eventProgressList[i];
                s += $"\t [{ i }] { progressData.id } - IsComplete: { progressData.progress >= progressData.goal } \n";
            }
        }

        return s;
    }
}

[System.Serializable]
public class EventProgressData
{
    public EventProgressData (string id, int progress, int goal)
    {
        this.id = id;
        this.progress = progress;
        this.goal = goal;
    }

    public string id;
    public int progress;
    public int goal;
}

[System.Serializable]
public class NavigationShipData 
{
    public bool initiated;

    public float posX;
    public float posY;
    public float angle;

    public Vector2 Position
    {
        get 
        {
            return new Vector2 (posX, posY);
        }
        set
        {
            initiated = true;

            posX = value.x;
            posY = value.y;
        }
    }
}