using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveFile
{
    public int version = 1;

    public float digitalCurrency;
    public float world1Currency;
    public float world2Currency;
    public float world3Currency;

    [SerializeField] public List<ItemIndexer> world1CurrencyIndexer;
    //public CurrencyItemIndexer world2CurrencyIndexer;
    //public CurrencyItemIndexer world3CurrencyIndexer;

    // ship customization data
    // ship bought items

    public List<AchievementLog> achievementLog;
    private float playtime;

    public SaveFile ()
    {
        world1CurrencyIndexer = new List<ItemIndexer>();
        //world2CurrencyIndexer = new CurrencyItemIndexer();
        //world3CurrencyIndexer = new CurrencyItemIndexer();

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
public class CurrencyItemIndexer 
{
    public CurrencyItemIndexer()
    {
        //list = new List<Vector3>();
    }

    //[SerializeField] public List<Vector3> list;
}

[System.Serializable]
public class ItemIndexer
{
    public ItemIndexer(float x, float y, float id)
    {
        v = new Vector3(x, y , id);
    }

    public Vector3 v;
}