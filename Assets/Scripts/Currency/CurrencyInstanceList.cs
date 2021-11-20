using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyInstanceList : MonoBehaviour
{
    static HashSet<Vector3> world1HashSet = new HashSet<Vector3>();
    static HashSet<Vector3> world2HashSet = new HashSet<Vector3>();
    static HashSet<Vector3> world3HashSet = new HashSet<Vector3>();

    private static HashSet<Vector3> GetWorldById (int worldId) 
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

    public static void CountAsCollected (int worldId, Vector3 objectId)
    {
        HashSet<Vector3> worldList = GetWorldById(worldId);
        worldList.Add(objectId);
    }

    public static bool CheckCollection (int worldId, Vector3 objectId)
    {
        HashSet<Vector3> worldList = GetWorldById(worldId);
        return worldList.Contains(objectId);
    }
}
