using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Minigame;

namespace Jumper 
{
    public class MJ_CameraController : MinigameCameraController
    {
        const float SCREEN_OFFSET = 3.25f;

        [SerializeField] float lerpRatio;
        [SerializeField] MJ_Player player;

        void Start()
        {
            if (!player)
            {
                enabled = false;
                return;
            }
        }

        void FixedUpdate()
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
    }

}
