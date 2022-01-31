using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFocusController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera staticCamera;
    [SerializeField] CinemachineVirtualCamera planetFocusCamera;
    [SerializeField] CinemachineVirtualCamera playerFocusCamera;

    public enum Focus { Static, Player, Planet }
    Focus currentFocus;

    public static CameraFocusController Instance;

    private void Awake() 
    {
        Instance = this;
    }

    public void SetStaticFocus()
    {
        //staticCamera.ForceCameraPosition(Vector3.zero, Quaternion.identity);

        staticCamera.Priority = 1;
        planetFocusCamera.Priority = 0;
        playerFocusCamera.Priority = 0;

        currentFocus = Focus.Static;
    }

    public void SetPlanetFocus (Transform target, float size = -1)
    {
        //Debug.Log("SetPlanetFocus");
        planetFocusCamera.Follow = target;
        if (size > 0)
            planetFocusCamera.m_Lens.OrthographicSize = size;
        
        staticCamera.Priority = 0;
        planetFocusCamera.Priority = 1;
        playerFocusCamera.Priority = 0;

        currentFocus = Focus.Planet;
    }

    public void SetPlayerFocus()
    {
        staticCamera.Priority = 0;
        planetFocusCamera.Priority = 0;
        playerFocusCamera.Priority = 1;    

        currentFocus = Focus.Player;
    }

    public void SetInstantPlayerFocus()
    {
        if (playerFocusCamera.Follow)
        {
            playerFocusCamera.transform.position =
                new Vector3(
                    playerFocusCamera.Follow.transform.position.x,
                    playerFocusCamera.Follow.transform.position.y,
                    staticCamera.transform.position.z
                );
        }

        SetPlayerFocus();
    }
}
