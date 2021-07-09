using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFocusController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera planetFocusCamera;
    [SerializeField] CinemachineVirtualCamera playerFocusCamera;
    [Space]
    [SerializeField] Transform startingFocus;

    public enum Focus { Player, Planet }
    Focus currentFocus;

    public static CameraFocusController Instance;

    private void Awake() 
    {
        Instance = this;

        if (startingFocus)
            SetPlanetFocus (startingFocus);
    }

    public void SetPlanetFocus (Transform target, float size = -1)
    {
        planetFocusCamera.Follow = target;
        if (size > 0)
            planetFocusCamera.m_Lens.OrthographicSize = size;
        
        planetFocusCamera.Priority = 1;
        playerFocusCamera.Priority = 0;

        currentFocus = Focus.Planet;
        //Debug.Log("Planet Focus");
    }

    public void SetPlayerFocus()
    {
        planetFocusCamera.Priority = 0;
        playerFocusCamera.Priority = 1;    

        currentFocus = Focus.Player;
        //Debug.Log("Player Focus");
    }
}
