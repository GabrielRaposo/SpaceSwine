﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Traveler
{
    public class MT_Player : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] Transform aimAnchor;
        [SerializeField] MT_Barrier barrier;

        bool invincible;

        Vector2 direction;
        Rigidbody2D rb;
        MT_ScreenLooper screenLooper;
        MT_BulletPool bulletPool;

        PlayerInputActions inputActions;
        InputAction movementAction;

        static public MT_Player Instance;

        private void Awake()
        {
            Instance = this;

            rb = GetComponent<Rigidbody2D>();
            screenLooper = GetComponent<MT_ScreenLooper>();
        }

        private void OnEnable()
        {
            inputActions = new PlayerInputActions();
            inputActions.Enable();

            movementAction = inputActions.Minigame.Movement;
            movementAction.Enable();

            inputActions.Minigame.Action.performed += (ctx) => 
            {
                ShootInput();
            };
            inputActions.Minigame.Action.Enable();
        }

        void Start()
        {
            bulletPool = MT_BulletPool.Instance;

            barrier.gameObject.SetActive(false);

            direction = Vector2.up;
            UpdateAimDisplay();
        }

        void UpdateAimDisplay()
        {
            if (!aimAnchor || !screenLooper.InsideZone)
                return;

            aimAnchor.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
        }

        void ShootInput()
        {
            if (rb.velocity.normalized == -direction.normalized)
                return;

            rb.velocity = speed * direction * -1;

            MT_Bullet bullet = bulletPool.GetMainBullet();
            bullet.Shoot (speed * direction * .5f, transform.position);
        }

        void Update()
        {
            Vector2 axisInput = movementAction.ReadValue<Vector2>();
        
            if (axisInput == Vector2.zero)
                return;

            direction = axisInput.normalized;
            UpdateAimDisplay();
        }

        private void OnDisable()
        {
            inputActions.Disable();

            movementAction.Disable();
            inputActions.Minigame.Action.Disable();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            MT_Collectable collectable = collision.GetComponent<MT_Collectable>();
            if (collectable)
            {
                collectable.OnCollect();
                barrier.Setup(transform);

                invincible = true;
                this.WaitSeconds(.1f, () => invincible = false);

                return;
            }

            MT_Bullet bullet = collision.GetComponent<MT_Bullet>();
            if (bullet && bullet.FriendlyFire && !invincible)
            {
                gameObject.SetActive(false);
                return;
            }
        }
    }

}