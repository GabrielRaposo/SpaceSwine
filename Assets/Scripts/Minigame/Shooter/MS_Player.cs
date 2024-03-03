using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Minigame;
using UnityEngine.Events;

namespace Shooter
{
    public class MS_Player : MonoBehaviour
    {
        [Header("Shooting")]
        [SerializeField] float shootSpeed;
        [SerializeField] int startingAmmo;

        [Header("Movement")]
        [SerializeField] float rotationSpeed;
        [SerializeField] Transform rotationAnchor;
        [SerializeField] float maxAngle;

        int ammo;
        bool hasMoved;

        MS_BulletPool bulletPool;
        MS_AmmoDisplay ammoDisplay;

        PlayerInputActions inputActions;
        InputAction axisInput;

        public UnityAction OnFirstMove;

        private void OnEnable()
        {
            inputActions = new PlayerInputActions();
            inputActions.Enable();

            axisInput = inputActions.Minigame.Movement;
            axisInput.Enable();

            inputActions.Minigame.Action.performed += ShootInput;
            inputActions.Enable();
        }

        private void Start()
        {
            bulletPool = MS_BulletPool.Instance;
            ammoDisplay = MS_AmmoDisplay.Instance;

            ammoDisplay.UpdateDisplay (ammo = startingAmmo);
        }
        private bool UseAmmo()
        {
            if (ammo < 0)
                return false;

            ammoDisplay.UpdateDisplay (--ammo);
            return true;
        }

        private void ShootInput (InputAction.CallbackContext ctx)
        {
            MS_Bullet bullet = bulletPool.Get();
            if (bullet == null) 
                return;

            if (!UseAmmo())
                return;

            Vector2 velocity = shootSpeed * RaposUtil.RotateVector (Vector2.up, rotationAnchor.eulerAngles.z);
            bullet.Shoot (this, transform.position, velocity);

            if (!hasMoved)
            {
                OnFirstMove?.Invoke();
                hasMoved = true;
            }
        }

        void Update()
        {
            if (!hasMoved)
                return;

            if (!rotationAnchor)
                return;

            Vector2 axis = axisInput.ReadValue<Vector2>();

            float angle = rotationAnchor.eulerAngles.z;
            if (axis.x != 0)
            {
                int direction = axis.x > 0 ? -1 : 1;
                angle += direction * rotationSpeed * Time.deltaTime;
                if (angle > 180)
                    angle -= 360;

                if (Mathf.Abs(angle) > maxAngle)
                    angle = maxAngle * direction;
            }

            rotationAnchor.eulerAngles = Vector3.forward * angle;
        }

        public void OnAmmoDestroyed()
        {
            Debug.Log("Ammo Broke");

            if (ammo < 0)
                Debug.Log("Die!");
        }

        private void OnDisable()
        {
            inputActions.Disable();
            axisInput.Disable();
            inputActions.Disable();
        }
    }
}
