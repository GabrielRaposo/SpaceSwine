using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyInstanceList : MonoBehaviour
{
    //static HashSet<Vector3> world1HashSet = new HashSet<Vector3>();
    //static HashSet<Vector3> world2HashSet = new HashSet<Vector3>();
    //static HashSet<Vector3> world3HashSet = new HashSet<Vector3>();

    static List<ItemIndexer> world1HashSet = new List<ItemIndexer>();
    static List<ItemIndexer> world2HashSet = new List<ItemIndexer>();
    static List<ItemIndexer> world3HashSet = new List<ItemIndexer>();

    public static void LoadSaveData()
    {
        SetWorldById(SaveManager.GetWorldHashSet(1), 1);
        SetWorldById(SaveManager.GetWorldHashSet(2), 2);
        SetWorldById(SaveManager.GetWorldHashSet(3), 3);
    }

    public static void SetWorldById (List<ItemIndexer> worldHashSet, int worldId) 
    { 
        switch (worldId)
        {
            default:
            case 1:
                world1HashSet = worldHashSet;
                break;

            case 2:
                world2HashSet = worldHashSet;
                break;

            case 3:
                world2HashSet = worldHashSet;
                break;
        }
    }

    public static List<ItemIndexer> GetWorldById (int worldId) 
    { 
        switch (worldId)
        {
            default:
            case 1:
                return world1HashSet;

            case 2:
                return world2HashSet;

            case 3:
                return world3HashSet;
        }
    }

    public static List<ItemIndexer> GetWorldFromSave(int worldId) 
    {
        switch (worldId) {
            default:
            case 1:
                world1HashSet = SaveManager.GetWorldHashSet(1);
                return world1HashSet;

            case 2:
                world2HashSet = SaveManager.GetWorldHashSet(2);
                return world2HashSet;

            case 3:
                world3HashSet = SaveManager.GetWorldHashSet(3);
                return world3HashSet;
        }
    }

    public static void CountAsCollected (int worldId, ItemIndexer objectId)
    {
        List<ItemIndexer> worldList = GetWorldById(worldId);
        worldList.Add(objectId);
    }

    public static bool CheckCollection (int worldId, ItemIndexer objectId)
    {
        List<ItemIndexer> worldList = GetWorldFromSave(worldId);

        if (worldList.Contains(objectId))
            return true;

        return false;
    }
}
