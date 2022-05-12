using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddingFlowerGroup : MonoBehaviour
{
    [SerializeField] Door door;

    int activeCount;
    List<BuddingFlower> flowers;
    
    void Awake()
    {
        flowers = new List<BuddingFlower>();
    }

    private void Start() 
    {    
        activeCount = 0;
        
        Round round = GetComponentInParent<Round>();
        if (round)
        {
            round.OnReset += ResetComponents;
        }
    }

    private void ResetComponents()
    {
        activeCount = 0;
    }

    public void AddFlower(BuddingFlower flower)
    {
        flowers.Add(flower);
    }

    public void Activate()
    {
        activeCount++;
        
        if (activeCount >= flowers.Count)
        {
            Debug.Log("All flowers are active");
            if (door)
            {
                door.TakeHealth();
            }
        }
    }
}
