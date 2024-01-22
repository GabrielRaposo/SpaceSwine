using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationSpaceStreet : MonoBehaviour
{
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 end;
    [SerializeField] float duration;
    [SerializeField] List<Sprite> shipSprites;

    void Start()
    {
        NavigationSpaceStreetShip[] ships = GetComponentsInChildren<NavigationSpaceStreetShip>();
        if (ships.Length < 1)
            return;

        for (int i = 0; i < ships.Length; i++)
        {
            float startCount = ((duration / ships.Length) * i) + ((duration / ships.Length) * Random.Range(0f, .9f));
            ships[i].Setup (transform.position + start, transform.position + end, duration, shipSprites[i % shipSprites.Count], startCount);
        }
    }

    private void OnDrawGizmos()
    {
        if (start == end)    
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + start, transform.position + end);
    }
}
