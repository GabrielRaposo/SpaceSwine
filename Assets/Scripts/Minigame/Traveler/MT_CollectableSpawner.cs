using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MT_CollectableSpawner : MonoBehaviour
{
    const int SIZE_X = 8;
    const int SIZE_Y = 7;

    [Header("Spawn")]
    [SerializeField] float spawnRatio;

    [Header("Pool")]
    [SerializeField] int poolSize;
    [SerializeField] GameObject collectableBase;

    List<MT_Collectable> collectables;

    int index;
    float spawnCount;

    static int Score;

    void Start()
    {
        if (poolSize < 1)
            return;

        collectables = new List<MT_Collectable>();
        
        for (int i = 0; i < poolSize; i++) 
        {
            MT_Collectable collectable = Instantiate(collectableBase, transform).GetComponent<MT_Collectable>();
            collectable.gameObject.SetActive(false);
            collectables.Add(collectable);
        }

        collectableBase.SetActive(false);
        index = 0;
        Score = 0;

        MT_ProgressDisplay.Instance.UpdateScore(Score);
    }

    void Update()
    {
        spawnCount += Time.deltaTime;

        if (spawnCount > spawnRatio)
        {
            SpawnCollectable();
            spawnCount = 0;
        }
    }

    private MT_Collectable GetCollectable()
    {
        MT_Collectable collectable = collectables[index % collectables.Count];
        index = (index + 1) % collectables.Count;

        return collectable;
    }

    private void SpawnCollectable()
    {
        Vector2 spawnPos = new Vector2 
        ( 
            Random.Range(-SIZE_X/2f, SIZE_X/2f),
            Random.Range(-SIZE_Y/2f, SIZE_Y/2f) 
        );

        MT_Collectable collectable = GetCollectable();
        collectable.Spawn(spawnPos);
    }

    public static void AddScore (int value)
    {
        Score += value;
        MT_ProgressDisplay.Instance.UpdateScore(Score);
    }
}
