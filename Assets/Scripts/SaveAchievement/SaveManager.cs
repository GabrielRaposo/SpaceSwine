using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveFile currentSave;

    readonly static string path = Application.persistentDataPath + "/astropig_new_save.save";

    private static int safety;
    static SaveManager()
    {
        Debug.Log("Save Manager Initiated!");
        Debug.Log("Path: " + path);

        currentSave = new SaveFile();
        currentSave = Load();

        safety = 0;

        //Debug.Log (currentSave.PrintStoredData());
    }

    public static SaveFile Load()
    {
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
            SaveFile loadedSaveFile = (SaveFile)bf.Deserialize(file);
            file.Close();

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

    public static void Save() 
    {
        FileStream file;

        if (File.Exists(path))
            file = File.OpenWrite(path);
        else
            file = File.Create(path);

        BinaryFormatter bf = new BinaryFormatter();

        bf.Serialize(file, currentSave);
        file.Close();
    }

    public static void ResetSave()
    {
        currentSave = new SaveFile();   
        Save();
        AchievementsManager.SetCurrentList(currentSave.achievementLog);
        Debug.Log ("Save Reseted.");
    }

    public static void SaveAllData()
    {
        Debug.Log("Update and save data");

        // Save currency
        currentSave.world1Currency = PlayerWallet.GetValueBy(1);
        //currentSave.world1CurrencyIndexer = CurrencyInstanceList.GetWorldById(1);

        // Save events
        // Save ship states

        Save();
    }

    //public static float GetCurrency(int id)
    //{
    //    switch(id)
    //    {
    //        default:
    //        case 0:
    //            return currentSave.digitalCurrency;

    //        case 1:
    //            return currentSave.world1Currency;
    //        case 2:
    //            return currentSave.world2Currency;
    //        case 3:
    //            return currentSave.world3Currency;
    //    }
    //}

    public static CurrencyItemIndexer GetWorldHashSet(int id)
    {
        return new CurrencyItemIndexer();

        //currentSave = Load();

        switch(id)
        {
            default:
                return new CurrencyItemIndexer();

            //case 1:
            //    return currentSave.world1CurrencyIndexer;
            //case 2:
            //    return currentSave.world2CurrencyIndexer;
            //case 3:
            //    return currentSave.world3CurrencyIndexer;
        }
    }

    public static float GetPlaytime()
    {
        return currentSave.GetPlaytime();
    }

    public static void AddPlaytime(float sessionTime)
    {
        currentSave.AddPlaytime(sessionTime);
        Save();
    }

    private static bool IsSaveValid(SaveFile file)
    {
        if (file == null) return false;
        if (file.version != new SaveFile().version) return false;
        if (file.achievementLog == null) return false;

        var systemAchievements = AchievementsManager.GetNewAchievementList();

        if (file.achievementLog.Count != systemAchievements.Count) return false;

        return true;
    }

    private static void SpecificAchievementsUpdate(SaveFile saveFile)
    {

    }
}
