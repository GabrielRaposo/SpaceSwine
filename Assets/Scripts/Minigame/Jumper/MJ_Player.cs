using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Jumper
{
    public class MJ_Player : MonoBehaviour
    {
        [SerializeField] float jumpForce;

        [Header("References")]
        [SerializeField] GameObject destroyAnimation;
        [SerializeField] MJ_GameManager gameManager;
        [SerializeField] ParticleSystem trailEffect;
        [SerializeField] ParticleSystem launchEffect;
        [SerializeField] ParticleSystem landingEffect;

        [Header("Audio")]
        [SerializeField] AK.Wwise.Event jumpAKEvent;
        [SerializeField] AK.Wwise.Event landAKEvent;
        [SerializeField] AK.Wwise.Event breakAKEvent;

        int difficultyIndex;
        bool outsideScreen;
        bool hasMoved;
        bool firstLanding;

        Rigidbody2D rb;
        PlayerInputActions playerInputActions;
        MJ_Planet landedOn;
        MJ_Planet previous;

        public UnityAction OnFirstMove;

        void OnEnable()
        {
            rb = GetComponent<Rigidbody2D>();

            playerInputActions = new PlayerInputActions();

            playerInputActions.Minigame.Action.performed += (ctx) => 
            {
                JumpOff();
            };
            playerInputActions.Minigame.Action.Enable();
        }

        private void Start() 
        {
            difficultyIndex = 1;
            MJ_Planet.ResetDifficulty();    
        }

        private void Update() 
        {
            if (HighestYPosition() > 25 * difficultyIndex)
            {
                MJ_Planet.AddDifficulty();
                difficultyIndex++;
            }

            if (outsideScreen && !landedOn)
            {
                Die();
                return;
            }    
        }

        private void JumpOff()
        {
            if (landedOn == null)
                return;

            if (!hasMoved)
            {
                OnFirstMove?.Invoke();
                hasMoved = true;
            }

            if (launchEffect)
            {
                float angle = Vector2.SignedAngle(Vector2.up, transform.up);
                ParticleSystem.MainModule mainModule = launchEffect.main;
                mainModule.startRotation = angle * Mathf.Deg2Rad * -1f;
                launchEffect.Play();
            }
            trailEffect?.Play();
            jumpAKEvent?.Post(gameObject);

            rb.velocity = transform.up * jumpForce * Time.fixedDeltaTime;

            landedOn.Dettach (transform);

            previous = landedOn;
            landedOn = null;
        }

        public float HighestYPosition()
        {
            float y = transform.position.y;

            if (landedOn && landedOn.transform.position.y > y)
                y = landedOn.transform.position.y;

            return y;
        }

        private void OnTriggerEnter2D (Collider2D collision) 
        {
            if (collision.CompareTag("GameplayArea"))
            {
                outsideScreen = false;
            }

            MJ_Planet planet = collision.GetComponent<MJ_Planet>();
            if (planet)
            {
                if (landedOn == null && (previous == null || previous != planet))
                {
                    trailEffect?.Pause();
                    if (firstLanding)
                        firstLanding = false;
                    else 
                        landAKEvent?.Post(gameObject);

                    if (hasMoved)
                        StartCoroutine ( RaposUtil.Wait(1, () => landingEffect?.Play() ) );

                    rb.velocity = Vector2.zero;

                    planet.Attach (transform);
                    landedOn = planet;
                }
            }

            //MJ_Hazard hazard = collision.GetComponent<MJ_Hazard>();
            if (collision.CompareTag("Hazard"))
            {
                Die();
            }
        }

        private void OnTriggerExit2D(Collider2D collision) 
        {
            if (collision.CompareTag("GameplayArea"))
            {
                outsideScreen = true;
            }
        }

        private void Die()
        {
            if (destroyAnimation)
            {
                breakAKEvent?.Post(gameObject);

                destroyAnimation.transform.SetParent(null);
                destroyAnimation.transform.eulerAngles = Vector3.zero;
                destroyAnimation.SetActive(true);
            }
            
            gameObject.SetActive(false);
            
            if (gameManager)
            {
                gameManager.ResetScene (.5f);
            }
        }

        private void OnDisable() 
        {
            playerInputActions.Minigame.Action.Disable();
        }
    }
}
