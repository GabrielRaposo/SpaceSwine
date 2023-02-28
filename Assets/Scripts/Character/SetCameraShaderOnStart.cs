using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraShaderOnStart : MonoBehaviour
{
    [SerializeField] LocalGameplayState gameplayState;

    void Awake()
    {
        if (!gameplayState)
            return;

        CameraShaderManager cameraShaderManager = CameraShaderManager.Instance;

        if(!cameraShaderManager)
            return;
        
        cameraShaderManager.SetDangerAreaShader(gameplayState.state == GameplayState.Danger);
    }
}
