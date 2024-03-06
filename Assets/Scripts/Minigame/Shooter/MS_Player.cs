using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Minigame;
using UnityEngine.Events;
using DG.Tweening.Core.Easing;

namespace Shooter
{
    public class MS_Player : MonoBehaviour
    {
        const int MAX_AMMO = 3;

        [Header("Shooting")]
        [SerializeField] float shotSpeed;
        [SerializeField] float shotCooldown;
        [SerializeField] int startingAmmo;
        [SerializeField] Transform shootPosition;

        [Header("Movement")]
        [SerializeField] float rotationSpeed;
        [SerializeField] Transform rotationAnchor;
        [SerializeField] float maxAngle;

        int ammo;
        int activeBullets;
        float cooldownCount;
        
        bool hasMoved;
        public static bool LostMatch {get; private set;} 

        MS_BulletPool bulletPool;
        MS_AmmoDisplay ammoDisplay;
        MinigameManager gameManager;

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
            gameManager = MinigameManager.Instance;

            ammoDisplay.UpdateDisplay (ammo = startingAmmo);

            LostMatch = false;
        }
        private bool UseAmmo()
        {
            if (ammo < 1)
                //return false;
                return true;

            ammoDisplay.UpdateDisplay (--ammo);
            return true;
        }

        private void ShootInput (InputAction.CallbackContext ctx)
        {
            if (MS_SessionManager.OnSessionTransition)
                return;

            if (cooldownCount > 0 || !UseAmmo())
                return;

            MS_Bullet bullet = bulletPool.Get();
            if (bullet == null) 
                return;

            activeBullets++;

            Vector2 velocity = shotSpeed * RaposUtil.RotateVector (Vector2.up, rotationAnchor.eulerAngles.z);
            bullet.Shoot (this, shootPosition.position, velocity);

            cooldownCount = shotCooldown;

            if (!hasMoved)
            {
                OnFirstMove?.Invoke();
                hasMoved = true;
            }
        }

        void Update()
        {
            if (cooldownCount > 0)
                cooldownCount -= Time.deltaTime;

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

        public void RestoreAmmo()
        {
            ammo++;

            if (ammo > MAX_AMMO)
                ammo = MAX_AMMO;

            ammoDisplay.UpdateDisplay(ammo);
        }

        public void OnAmmoDestroyed()
        {
            if (--activeBullets > 0 || ammo > 0)
                return;
            
            //Die();
        }

        public void ClearActiveBullets()
        {
            bulletPool.VanishAllBullets();
            activeBullets = 0;
        }

        public void Die()
        {
            //if (destroyAnimation)
            //{
            //    breakAKEvent?.Post(gameObject);

            //    destroyAnimation.transform.SetParent (null);
            //    destroyAnimation.transform.eulerAngles = Vector3.zero;
            //    destroyAnimation.SetActive (true);
            //}
            
            gameObject.SetActive(false);
            LostMatch = true;

            if (gameManager)
                gameManager.ResetScene(.5f);
        }


        private void OnDisable()
        {
            inputActions.Disable();
            axisInput.Disable();
            inputActions.Disable();
        }
    }
}
