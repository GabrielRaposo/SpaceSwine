using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyInstanceList : MonoBehaviour
{
    static List<Vector3> world1CollectedItems = new List<Vector3>();
    //static List<Vector2> world2CollectedItems = new List<Vector2>();
    //static List<Vector2> world3CollectedItems = new List<Vector2>();

    static HashSet<Vector3> world1HashSet = new HashSet<Vector3>();

    private static List<Vector3> GetWorldById(int worldId) 
    { 
        switch (worldId)
        {
            default:
            case 1:
                return world1CollectedItems;
        }
    }

    public static void CountAsCollected(int worldId, Vector3 objectId)
    {
        List<Vector3> worldList = GetWorldById(worldId);
        worldList.Add(objectId);
    }

    public static bool CheckCollection(int worldId, Vector3 objectId)
    {
        List<Vector3> worldList = GetWorldById(worldId);
        return worldList.Find((w) => w == objectId) == objectId;
    }
}
