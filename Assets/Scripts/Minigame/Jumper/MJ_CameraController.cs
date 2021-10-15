using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Jumper 
{
    public class MJ_CameraController : MonoBehaviour
    {
        const float SCREEN_OFFSET = 3.25f;

        [SerializeField] float lerpRatio;
        [SerializeField] MJ_Player player;

        public static MJ_CameraController Instance;

        private void Awake() 
        {
            Instance = this;
        }

        void Start()
        {
            if (!player)
            {
                enabled = false;
                return;
            }
        }

        void Update()
        {
            float targetY = player.HighestYPosition();

            //if (targetY > transform.position.y - SCREEN_OFFSET )
            //{
                transform.position = new Vector3
                (
                    transform.position.x, 
                    Mathf.Lerp(transform.position.y, targetY + SCREEN_OFFSET, lerpRatio),
                    transform.position.z
                );
            //}
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
