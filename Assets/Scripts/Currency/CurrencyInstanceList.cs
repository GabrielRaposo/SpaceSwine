using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyInstanceList : MonoBehaviour
{
    //static HashSet<Vector3> world1HashSet = new HashSet<Vector3>();
    //static HashSet<Vector3> world2HashSet = new HashSet<Vector3>();
    //static HashSet<Vector3> world3HashSet = new HashSet<Vector3>();

    static List<ItemIndexer> world1IndexerList = new List<ItemIndexer>();
    static List<ItemIndexer> world2IndexerList = new List<ItemIndexer>();
    static List<ItemIndexer> world3IndexerList = new List<ItemIndexer>();

    static bool initiated;

    public static void Init()
    {
        if (initiated)
            return;

        SetWorldById(SaveManager.GetWorldHashSet(1), 1);
        SetWorldById(SaveManager.GetWorldHashSet(2), 2);
        SetWorldById(SaveManager.GetWorldHashSet(3), 3);

        initiated = true;
    }

    public static void SetWorldById (List<ItemIndexer> worldHashSet, int worldId) 
    { 
        switch (worldId)
        {
            default:
            case 1:
                world1IndexerList = worldHashSet;
                break;

            case 2:
                world2IndexerList = worldHashSet;
                break;

            case 3:
                world2IndexerList = worldHashSet;
                break;
        }
    }

    public static List<ItemIndexer> GetWorldById (int worldId) 
    { 
        //string s = "World 1: \n";
        //foreach(ItemIndexer ii in world1IndexerList)
        //{
        //    s += $"ii: { ii.ToString() } \n";
        //}
        //Debug.Log(s);

        switch (worldId)
        {
            default:
            case 1:
                return world1IndexerList;

            case 2:
                return world2IndexerList;

            case 3:
                return world3IndexerList;
        }
    }

    public static List<ItemIndexer> GetWorldFromSave(int worldId) 
    {
        switch (worldId) {
            default:
            case 1:
                world1IndexerList = SaveManager.GetWorldHashSet(1);
                return world1IndexerList;

            case 2:
                world2IndexerList = SaveManager.GetWorldHashSet(2);
                return world2IndexerList;

            case 3:
                world3IndexerList = SaveManager.GetWorldHashSet(3);
                return world3IndexerList;
        }
    }

    public static void CountAsCollected (int worldId, ItemIndexer objectId)
    {
        List<ItemIndexer> worldList = GetWorldById(worldId);
        worldList.Add(objectId);
    }

    public static bool CheckCollection (int worldId, ItemIndexer objectId)
    {
        Init();

        List<ItemIndexer> worldList = GetWorldById(worldId);

        //string s = "Check for objectId: " + objectId.ToString() + "\n";
        //foreach(ItemIndexer ii in world1IndexerList)
        //{
        //    s += $"ii: {ii.ToString()} \n";
        //}
        //Debug.Log(s);

        if (worldList.Find((o) => o.CompareTo(objectId)) != null)
            return true;

        //Debug.Log("No correlation found");
        return false;
    }
}
