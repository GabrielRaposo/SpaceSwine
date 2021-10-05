using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSizeController : MonoBehaviour
{
    [SerializeField] List<CinemachineVirtualCamera> vcams;

    float size = -1f;
    
    private static CameraSizeController Instance;

    private void Awake() 
    {
        Instance = this;    
    }

    public static float Size
    {
        get 
        {
            if (Instance)
                return Instance.size;

            return -1;
        }

        set
        {
            if (Instance)
                Instance.SetSizeLocal(value);
        }
    }

    //public static float GetSize()
    //{
    //    if (Instance)
    //        return Instance.size;

    //    return -1;
    //}

    //public static void SetSize(float size)
    //{
    //    if (Instance)
    //        Instance.SetSizeLocal(size);
    //}

    private void SetSizeLocal (float size)
    {
        if (vcams.Count < 1)
            return;

        foreach(CinemachineVirtualCamera vcam in vcams)
        {
            vcam.m_Lens.OrthographicSize = size;
        }

        this.size = size; 
    }

}
