using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jumper
{
    public class MJ_Player : MonoBehaviour
    {
        [SerializeField] float jumpForce;

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

            if (landedOn)
            {
                y = landedOn.transform.position.y;
            }

            return y;
        }

        private void OnTriggerEnter2D (Collider2D collision) 
        {
            if (landedOn == null && (previous == null || previous != landedOn))
            {
                rb.velocity = Vector2.zero;

                MJ_Planet planet = collision.GetComponent<MJ_Planet>();
                if (!planet)
                    return;

                planet.Attach (transform);
                landedOn = planet;
            }
        }

        private void OnTriggerExit2D(Collider2D collision) 
        {
            if (!collision.CompareTag("GameplayArea"))
                return;

            SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
        }

        private void OnDisable() 
        {
            playerInputActions.Minigame.Action.Disable();
        }
    }
}
