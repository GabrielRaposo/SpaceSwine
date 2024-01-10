using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationInteractableGlimmer : MonoBehaviour
{
    public float spinMultiplier;
    public float spinDuration;

    float spinCount;
    int direction = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (spinCount > 0)
            return;

        NavigationShip ship = collision.GetComponent<NavigationShip>();
        if (!ship) 
            return;

        spinCount = spinDuration;
        direction = Random.Range(0, 2) == 0 ? 1 : -1;  
    }

    private void Start()
    {
        transform.Rotate (Vector3.forward * Random.Range(0, 45));
    }

    private void Update()
    {
        if (spinCount < 0)
            return;

        transform.Rotate ( Vector3.forward * (spinMultiplier * (spinCount/spinDuration) * direction) );
        spinCount -= Time.deltaTime;
    }
}
