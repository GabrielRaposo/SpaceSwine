using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jumper
{
    public class MJ_Player : MonoBehaviour
    {
        [SerializeField] float jumpForce;

        bool outsideScreen;

        Rigidbody2D rb;
        PlayerInputActions playerInputActions;
        MJ_Planet landedOn;
        MJ_Planet previous;

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

        private void Update() 
        {
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
                    rb.velocity = Vector2.zero;

                    planet.Attach (transform);
                    landedOn = planet;
                }
            }

            MJ_Hazard hazard = collision.GetComponent<MJ_Hazard>();
            if (hazard)
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
            SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
            gameObject.SetActive(false);
        }

        private void OnDisable() 
        {
            playerInputActions.Minigame.Action.Disable();
        }
    }
}
