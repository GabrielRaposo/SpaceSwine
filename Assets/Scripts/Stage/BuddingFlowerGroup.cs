using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddingFlowerGroup : MonoBehaviour
{
    [SerializeField] Door door;
    [SerializeField] float activationDelay;

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
            StartCoroutine( ActivationRoutine() );
        }
    }

    private IEnumerator ActivationRoutine()
    {
        foreach(BuddingFlower bf in flowers)
            bf.PreLightUp();

        yield return new WaitForSeconds(activationDelay);

        foreach(BuddingFlower bf in flowers)
            bf.LightUp();

        if (door)
        {
            door.TakeHealth();
        }
    }

    public Vector3 DoorPosition()
    {
        return door.transform.position;
    }
}
