using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNavigationShipPositionOnStart : MonoBehaviour
{
    [SerializeField] Vector3 position;    

    void Start()
    {
        NavigationShip.SetPreviousPosition(position);
    }
}
