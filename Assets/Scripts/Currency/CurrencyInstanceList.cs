using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyInstanceList : MonoBehaviour
{
    //static HashSet<Vector3> world1HashSet = new HashSet<Vector3>();
    //static HashSet<Vector3> world2HashSet = new HashSet<Vector3>();
    //static HashSet<Vector3> world3HashSet = new HashSet<Vector3>();

    static CurrencyItemIndexer world1HashSet = new CurrencyItemIndexer();
    static CurrencyItemIndexer world2HashSet = new CurrencyItemIndexer();
    static CurrencyItemIndexer world3HashSet = new CurrencyItemIndexer();

    public static void LoadSaveData()
    {
        SetWorldById(SaveManager.GetWorldHashSet(1), 1);
        SetWorldById(SaveManager.GetWorldHashSet(2), 2);
        SetWorldById(SaveManager.GetWorldHashSet(3), 3);
    }

    public static void SetWorldById (CurrencyItemIndexer worldHashSet, int worldId) 
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

    public static CurrencyItemIndexer GetWorldById (int worldId) 
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

    //public static CurrencyItemIndexer GetWorldFromSave (int worldId) 
    //{ 
    //    switch (worldId)
    //    {
    //        default:
    //        case 1:
    //            world1HashSet = SaveManager.GetWorldHashSet(1);
    //            return world1HashSet;

    //        case 2:
    //            world2HashSet = SaveManager.GetWorldHashSet(2);
    //            return world2HashSet;

    //        case 3:
    //            world3HashSet = SaveManager.GetWorldHashSet(3);
    //            return world3HashSet;
    //    }
    //}

    public static void CountAsCollected (int worldId, Vector3 objectId)
    {
        CurrencyItemIndexer worldList = GetWorldById(worldId);
        //worldList.list.Add(objectId);
    }

    public static bool CheckCollection (int worldId, Vector3 objectId)
    {
        return false;

        //CurrencyItemIndexer worldList = GetWorldFromSave(worldId);

        //if (worldList.list.Contains(objectId))
        //    return true;

        //return false;
    }
}
