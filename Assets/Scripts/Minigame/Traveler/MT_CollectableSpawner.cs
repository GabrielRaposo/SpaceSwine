using System.Collections;
using System.Collections.Generic;
using Traveler;
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

    int quad1Ratio;
    int quad2Ratio;
    int quad3Ratio;
    int quad4Ratio;

    int index;
    float spawnCount;

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

        quad1Ratio = quad2Ratio = quad3Ratio = quad4Ratio = 25;

        //collectableBase.SetActive(false);
        index = 0;
    }

    void Update()
    {
        if (!MT_Player.HasMoved)
            return;

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
        int quad = GetRandomQuad();
        Vector2 quadRange = GetQuadRange(quad);

        Vector2 spawnPos = new Vector2 
        ( 
            Random.Range(0f, quadRange.x),
            Random.Range(0f, quadRange.y) 
        );
        //Debug.Log($"quad: {quad}, spawnPos: {spawnPos}");

        UpdateRatio (quad);

        MT_Collectable collectable = GetCollectable();
        collectable.Spawn(spawnPos);
    }

    private int GetRandomQuad()
    {
        int r = Random.Range(0, quad1Ratio + quad2Ratio + quad3Ratio + quad4Ratio);
        //Debug.Log($"quad1: {quad1Ratio}, quad2: {quad2Ratio}, quad3: {quad3Ratio}, quad4: {quad4Ratio}");

        if (r < quad1Ratio)
            return 1;
        if (r < quad1Ratio + quad2Ratio)
            return 2;
        if (r < quad1Ratio + quad2Ratio + quad3Ratio)
            return 3;
        return 4;
    }

    private Vector2 GetQuadRange(int index)
    {
        switch (index)
        {
            default: 
            case 1:  return new Vector2 (-SIZE_X/2f, -SIZE_Y/2f);
            case 2:  return new Vector2 ( SIZE_X/2f, -SIZE_Y/2f);
            case 3:  return new Vector2 (-SIZE_X/2f,  SIZE_Y/2f);
            case 4:  return new Vector2 ( SIZE_X/2f,  SIZE_Y/2f);
        }
    }

    private void UpdateRatio (int quad)
    {
        quad1Ratio += (quad == 1) ? -3 : 1;
        quad2Ratio += (quad == 2) ? -3 : 1;
        quad3Ratio += (quad == 3) ? -3 : 1;
        quad4Ratio += (quad == 4) ? -3 : 1;
    }
}
