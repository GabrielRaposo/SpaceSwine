using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigame
{
    public class MinigameCameraController : MonoBehaviour
    {
        public static MinigameCameraController Instance;

        private void Awake() 
        {
            Instance = this;
        }

        public void SetRenderTexture (RenderTexture renderTexture)
        {
            Camera camera = GetComponent<Camera>();
            if (!camera)
                return;

            camera.targetTexture = renderTexture;
        }
    }
}
