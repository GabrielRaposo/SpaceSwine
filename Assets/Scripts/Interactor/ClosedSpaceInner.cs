using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedSpaceInner : MonoBehaviour
{
    [SerializeField] SpriteRenderer visualComponent;
    [SerializeField] GravitationalPlatform gravitationalPlatform;

    void Start()
    {
        SetState(false);
    }

    public void SetState (bool value)
    {
        visualComponent.enabled = value;
        gravitationalPlatform.gameObject.SetActive(value);
    }

    public GravitationalBody GetGravitationalBody
    {
        get 
        {
            if (!gravitationalPlatform)
                return null;

            return gravitationalPlatform;
        }
    }
}
