using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] List<Vector3> points;
    
    public static SpawnManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public Vector3 GetSpawnPoint ()
    {
        Vector3 point = Vector3.zero;

        if (points != null && points.Count > 0)
        {
            int index = 0;

            if (SaveManager.Initiated)
                index = SaveManager.GetSpawnData().spawnIndex;

            point = points[index % points.Count];
        }

        return point;
    }


    private void OnDrawGizmosSelected() 
    {
        if (points == null || points.Count < 1)
            return;

        Gizmos.color = Color.yellow;
        foreach (Vector3 v in points)
        {
            Gizmos.DrawWireSphere(v, .1f);
        }
    }
}
