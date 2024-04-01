using Minigame;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Traveler
{
    public class MT_Player : MonoBehaviour
    {
        const int SCREEN_X = 10;
        const int SCREEN_Y = 9;
        const float OFFSET = .35f;

        [SerializeField] float speed;
        [SerializeField] float hitStunDuration;
        [SerializeField] Transform visualComponent;
        [SerializeField] GameObject OnHitEffect;
        [SerializeField] SpriteRenderer signaler;
        [SerializeField] Transform aimAnchor;
        [SerializeField] MT_Barrier barrier;
        [SerializeField] MinigameManager gameManager;
        [SerializeField] CustomTimeCounter enduranceTimeCounter;

        [Header("Audio")]
        [SerializeField] AK.Wwise.Event shootAKEvent;
        [SerializeField] AK.Wwise.Event deathAKEvent;

        bool invincible;
        bool inputLocked;

        int offscreenFrameCount;

        Vector2 direction;
        Rigidbody2D rb;
        MT_ScreenLooper screenLooper;
        MT_BulletPool bulletPool;

        PlayerInputActions inputActions;
        InputAction movementAction;

        public UnityAction OnFirstMove;

        static public bool HasMoved {get; private set;}
        static public bool HasLost {get; private set;}

        static public MT_Player Instance;

        private void Awake()
        {
            Instance = this;

            rb = GetComponent<Rigidbody2D>();
            screenLooper = GetComponent<MT_ScreenLooper>();

            HasMoved = HasLost = false;
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

            direction = Vector2.right;
            aimAnchor.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
            UpdateAimDisplay();

            signaler.enabled = false;

            if (enduranceTimeCounter)
            {
                enduranceTimeCounter.OnTimeReached += (i) => 
                {
                    if (!AchievementsManager.GetAchievementState(AchievementEnum.LooperExtra))
                        AchievementsManager.SetAchievementState(AchievementEnum.LooperExtra, true);
                };
            }
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

            if (shootAKEvent != null)
                shootAKEvent.Post(gameObject);

            rb.velocity = speed * direction * -1;
            visualComponent.eulerAngles = Vector3.forward * (Vector2.SignedAngle(Vector2.up, direction) + 180);

            MT_Bullet bullet = bulletPool.GetMainBullet();
            bullet.Shoot (speed * direction * .5f, transform.position);

            if (!HasMoved)
            {
                OnFirstMove?.Invoke();
                HasMoved = true;

                if (enduranceTimeCounter)
                    enduranceTimeCounter.Restart();

                inputLocked = true;
                this.WaitSeconds(.5f, () => inputLocked = false);
            }
        }

        void Update()
        {
            if (!HasMoved || inputLocked)
                return;

            SignalerLogic();

            Vector2 axisInput = movementAction.ReadValue<Vector2>();
        
            if (axisInput == Vector2.zero)
                return;

            direction = axisInput.normalized;
            UpdateAimDisplay();
        }

        private void SignalerLogic()
        {
            if (IsOffscreen())
                offscreenFrameCount++;
            else
                offscreenFrameCount = 0;

            signaler.enabled = offscreenFrameCount > 30;
        }

        private bool IsOffscreen()
        {
            if (transform.localPosition.x < -(SCREEN_X/2) - OFFSET )
                return true;

            if (transform.localPosition.x >  (SCREEN_X/2) + OFFSET )
                return true;

            if (transform.localPosition.y < -(SCREEN_Y/2) - OFFSET )
                return true;

            if (transform.localPosition.y >  (SCREEN_Y/2) + OFFSET )
                return true;

            return false;
        }

        private void OnDisable()
        {
            inputActions.Disable();

            movementAction.Disable();
            inputActions.Minigame.Action.Disable();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (HasLost)
                return;

            MT_Collectable collectable = collision.GetComponent<MT_Collectable>();
            if (collectable)
            {
                collectable.OnCollect();
                barrier.Setup(transform);

                if (enduranceTimeCounter)
                    enduranceTimeCounter.Stop();

                invincible = true;
                this.WaitSeconds(1.5f, () => invincible = false);

                return;
            }

            MT_Bullet bullet = collision.GetComponent<MT_Bullet>();
            if (bullet && bullet.FriendlyFire && !invincible)
            {
                bullet.Highlight();
                if (OnHitEffect)
                {
                    Vector2 pos = transform.position + (bullet.transform.position - transform.position)/2f;
                    OnHitEffect.transform.position = pos;
                    OnHitEffect.SetActive(true);
                }
                Die();
                return;
            }
        }

        private void Die()
        {
            if (enduranceTimeCounter)
                enduranceTimeCounter.Stop();

            enabled = false;
            HasLost = true;

            if (OnHitEffect)
                OnHitEffect.SetActive(true);

            if (deathAKEvent != null)
                deathAKEvent.Post(gameObject);

            rb.velocity = Vector3.zero;
            aimAnchor.gameObject.SetActive(false);

            bulletPool.StopAllBullets();

            this.WaitSeconds(hitStunDuration, () => 
            {
                if (gameManager)
                    gameManager.ResetScene();
            });
        }
    }

}
