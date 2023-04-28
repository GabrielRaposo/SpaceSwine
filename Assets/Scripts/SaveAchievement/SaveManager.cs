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
        Debug.Log("Save Manager Initiated!");

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
            Debug.Log("File found!");
            
            FileStream file = File.OpenRead(path);
            BinaryFormatter bf = new BinaryFormatter();
            SaveFile loadedSaveFile = (SaveFile) bf.Deserialize(file);
            file.Close();

            loadedSaveFile.PrepFile();

            if (!IsSaveValid(loadedSaveFile)) 
            {
                Debug.LogWarning("Invalid save file!");
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
                SpecificAchievementsUpdate(currentSave);
                
                Save();

                loadedSaveFile = currentSave;
            }
            
            AchievementsManager.SetCurrentList(loadedSaveFile.achievementLog);

            safety = 0;
            return loadedSaveFile;
        }

        Debug.Log("File not found, creating new.");
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

        Debug.Log (currentSave.PrintStoredData());
    }

    public static void ResetSave()
    {
        currentSave = new SaveFile();   
        Save();

        StoryEventsManager.ReloadEventsDictionary();
        StoryEventsManager.UpdatePrintedEventStates();

        AchievementsManager.SetCurrentList(currentSave.achievementLog);

        Debug.Log ("Save Reseted.");
    }

    public static void SaveAllData()
    {
        Debug.Log ("Update and save data");
        Save();
    }

    #region Story Events
    public static List<EventProgressData> GetStoryEvents()
    {
        return currentSave.eventProgressList;
    }

    public static void SetStoryEvents (List<EventProgressData> progressList)
    {
        currentSave.eventProgressList = progressList;

        Save();
    }
    #endregion

    #region UI Notifications
    public static List<UINotification> GetUINotifications()
    {
        return currentSave.uiNotificationsList;
    }

    public static void SetUINotifications(List<UINotification> notificationsList)
    {
        currentSave.uiNotificationsList = notificationsList;

        Save();
    }
    #endregion

    #region Playtime
    public static float GetPlaytime()
    {
        return currentSave.Playtime;
    }

    public static void AddPlaytime (float sessionTime)
    {
        currentSave.Playtime += sessionTime;
        Save();
    }
    #endregion

    private static void SpecificAchievementsUpdate(SaveFile saveFile)
    {

    }
}
