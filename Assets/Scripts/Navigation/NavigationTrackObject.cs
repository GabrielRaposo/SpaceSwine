using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationTrackObject : MonoBehaviour
{
    [SerializeField] float speed;

    CinemachineTrack track;

    void Start()
    {
        track = GetComponent<CinemachineTrack>();
    }

    void Update()
    {
        //track.
    }
}
