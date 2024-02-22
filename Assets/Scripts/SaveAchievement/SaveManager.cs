using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static bool Initiated;

    private static SaveFile currentSave;
    readonly static string path = Application.persistentDataPath + "/astropig_new_save.save";
    private static int safety;

    #region Setup
    static SaveManager()
    {
        currentSave = Load();

        safety = 0;
        Initiated = true;
    }

    public static SaveFile Load()
    {
        SaveIcon.Show (Color.red);

        safety++;
        if (safety > 50)
        {
            Debug.LogError("Error loading save file");
            Application.Quit();
        }
        
        if (File.Exists(path))
        {
            DebugDisplay.Call("File found!");
            
            FileStream file = File.OpenRead(path);
            BinaryFormatter bf = new BinaryFormatter();
            SaveFile loadedSaveFile = (SaveFile) bf.Deserialize(file);
            file.Close();

            loadedSaveFile.PrepFile();

            if (!IsSaveValid(loadedSaveFile)) 
            {
                DebugDisplay.Call("Invalid save file!");
                currentSave = new SaveFile();

                if (loadedSaveFile.achievementLog != null)
                {
                    //Transfer achievements

                    foreach (AchievementLog currentEmptyAchievement in currentSave.achievementLog)
                    {
                        var loadedAchievement = loadedSaveFile.achievementLog.Find(x => x.Achievement.id == currentEmptyAchievement.Achievement.id);
                        if (loadedAchievement == null) continue;

                        Debug.Log($"Transfering: {loadedAchievement.Achievement.id}");
                        currentEmptyAchievement.achieved = loadedAchievement.achieved;
                        currentEmptyAchievement.progress = loadedAchievement.progress;

                        if (currentEmptyAchievement.Achievement.achievementType == AchievementType.Events)
                        {
                            if (loadedAchievement.Achievement.achievementType == AchievementType.Events)
                            {
                                foreach (AchievementEvent currentAchievementEvent in currentEmptyAchievement.events)
                                {
                                    var loadedEvent = loadedAchievement.events.Find(x=> x.code == currentAchievementEvent.code);
                                    if (loadedEvent != null)
                                        currentAchievementEvent.concluded = loadedEvent.concluded;
                                }
                            }
                        }
                    }
                }
                
                //Transfer progress
                // currentSave.SetProgress(loadedSaveFile.Progress);
                // currentSave.Playtime = loadedSaveFile.Playtime;
                
                AchievementsManager.SetCurrentList(currentSave.achievementLog);
                //SpecificAchievementsUpdate(currentSave);
                
                Save();

                loadedSaveFile = currentSave;
            }
            
            AchievementsManager.SetCurrentList(loadedSaveFile.achievementLog);

            safety = 0;
            return loadedSaveFile;
        }

        DebugDisplay.Call("File not found, creating new.");
        currentSave = new SaveFile();
        Save();

        return Load();
    }

    private static bool IsSaveValid (SaveFile file)
    {
        if (file == null) return false;
        if (file.version != new SaveFile().version) return false;
        if (file.achievementLog == null) return false;

        var systemAchievements = AchievementsManager.GetNewAchievementList();

        if (file.achievementLog.Count != systemAchievements.Count) return false;

        return true;
    }
    #endregion

    public static void Save() 
    {
        SaveIcon.Show (Color.white);

        FileStream file;

        if (File.Exists(path))
            file = File.OpenWrite(path);
        else
            file = File.Create(path);

        BinaryFormatter bf = new BinaryFormatter();

        bf.Serialize(file, currentSave);
        file.Close();

        //Debug.Log (currentSave.PrintStoredData());
    }

    public static void ResetSave()
    {
        currentSave = new SaveFile();   
        Save();

        UINotificationManager.ResetList();
        StoryEventsManager.ReloadEventsDictionary();
        StoryEventsManager.UpdatePrintedEventStates();
        AdventureLogManager.ResetStates();

        AchievementsManager.SetCurrentList (currentSave.achievementLog);
    }

    #region Spawn Data
    public static (string scenePath, int spawnIndex) GetSpawnData ()
    {
        return (currentSave.spawnScenePath, currentSave.spawnIndex);
    }

    public static void SetSpawnPath (string scenePath, bool autoSave = true)
    {
        currentSave.spawnScenePath = scenePath;

        if (autoSave)
            Save();
    }

    public static void SetSpawnIndex (int spawnIndex, bool autoSave = true)
    {
        currentSave.spawnIndex = spawnIndex;

        if (autoSave)
            Save();
    }
    #endregion

    #region Navigation Data

    public static int CurrentWorld
    {
        get { return currentSave.currentWorld;  }
        set { currentSave.currentWorld = value; }
    }

    public static (bool initiated, Vector2 position, float angle) GetNavigationData()
    {
        return (currentSave.navigationShipData.initiated, currentSave.navigationShipData.Position, currentSave.navigationShipData.angle);
    }

    public static void SetNavigationData (Vector2 position, float angle, bool autoSave = true)
    {
        currentSave.navigationShipData.Position = position;
        currentSave.navigationShipData.angle = angle;

        if (autoSave)
            Save();
    }

    public static string ShuttleExitLocationPath
    {
        get { return currentSave.shuttleExitLocationPath;  }
        set 
        {   
            currentSave.shuttleExitLocationPath = value; 
            Save();
        }
    }
    #endregion

    #region Minigame Highscore

    public static void SetHighscore (string minigame, int score)
    {
        bool isListed = false;

        if (currentSave.minigameHighscores.Count > 0)
        {
            foreach (var d in currentSave.minigameHighscores)
            {
                if (d.minigame == minigame)
                {
                    d.score = score;
                    isListed = true;
                    break;
                }
            }
        }

        if (!isListed)
            currentSave.minigameHighscores.Add(new MinigameHighscore(minigame, score));

        Save();
    }

    public static int GetHighscore (string minigame)
    {
        if (currentSave.minigameHighscores.Count < 1)
            return -1;

        foreach (var data in currentSave.minigameHighscores)
        {
            if (data.minigame == minigame)
                return data.score;
        }

        return -1; 
    }

    #endregion

    #region Story Events
    public static List<EventProgressData> GetStoryEvents()
    {
        return currentSave.eventProgressList;
    }

    public static void SetStoryEvents (List<EventProgressData> progressList, bool autoSave = true)
    {
        currentSave.eventProgressList = progressList;

        if (autoSave)
            Save();
    }

    public static void AddShipTalkEvent(StoryEventScriptableObject storyEvent)
    {
        if (currentSave.pendingShipDialogs == null)
            currentSave.pendingShipDialogs = new List<string>();
        
        string localizationID = "CHAT_W" + (int)storyEvent.worldTag + "_" + storyEvent.idTag + "_001.01";

        bool success;
        int n = 1;

        do
        {
            localizationID = localizationID.Remove(localizationID.Length - 4, 1);
            localizationID = localizationID.Insert(localizationID.Length - 3, n.ToString());
            
            success = LocalizationManager.GetShipText(localizationID).Item1;

            if (!success)
            {
                Debug.Log($"Skipped add {localizationID}");
                break;
            }

            Debug.Log($"Added {localizationID}");
            
            if(!currentSave.pendingShipDialogs.Contains(localizationID))
                currentSave.pendingShipDialogs.Add(localizationID);
            
            n++;

        } while (true);
        
        if (ShipInitializerSystem.Instance != null)
            ShipInitializerSystem.Instance.UpdateDialogButton();

        Save();
    }

    public static List<string> GetShipTalkIds()
    {
        if (currentSave.pendingShipDialogs == null)
            currentSave.pendingShipDialogs = new List<string>();

        return currentSave.pendingShipDialogs;
    }

    public static bool IsShipDialogListEmpty()
    {
        if (currentSave == null)
            return true;
        
        if (currentSave.pendingShipDialogs == null)
            return true;

        return currentSave.pendingShipDialogs.Count == 0;
    }

    public static void RemoveFromShipTalkIds(string id)
    {
        if (currentSave.pendingShipDialogs == null)
            currentSave.pendingShipDialogs = new List<string>();

        Debug.Log($"Removed {id} from ship dialog list");
        currentSave.pendingShipDialogs.Remove(id);
        
        if (ShipInitializerSystem.Instance != null)
            ShipInitializerSystem.Instance.UpdateDialogButton();
        
        Save();
    }
    
    #endregion

    #region UI Notifications
    public static List<UINotification> GetUINotifications()
    {
        return currentSave.uiNotificationsList;
    }

    public static void SetUINotifications(List<UINotification> notificationsList, bool autoSave = true)
    {
        currentSave.uiNotificationsList = notificationsList;

        if (autoSave)
            Save();
    }
    #endregion

    #region Playtime
    public static float GetPlaytime()
    {
        return currentSave.Playtime;
    }

    public static void AddPlaytime (float sessionTime, bool autoSave = true)
    {
        currentSave.Playtime += sessionTime;

        if (autoSave)
            Save();
    }
    #endregion
}
