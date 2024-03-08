using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MT_ScreenLooper : MonoBehaviour
{
    [SerializeField] float threshold = .5f;

    const int SCREEN_X = 10;
    const int SCREEN_Y = 9;

    [HideInInspector] public bool InsideZone;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GameplayArea"))
        {
            InsideZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (InsideZone && collision.CompareTag("GameplayArea"))
        {
            InsideZone = false;
            ScreenLoop();
        }
    }

    private void Update()
    {
        if (InsideZone)
            return;

        ScreenLoop();
    }

    private void ScreenLoop()
    {
        //Debug.Log("transform.localPosition: " + transform.localPosition);

        if (transform.localPosition.x - threshold > SCREEN_X / 2)
        {
            Debug.Log("A");
            transform.position += Vector3.left * (SCREEN_X + (threshold * 2));
            //insideZone = true;
            return;
        }

        if (transform.localPosition.x + threshold < -SCREEN_X / 2)
        {
            Debug.Log("B");
            transform.position += Vector3.right * (SCREEN_X + (threshold * 2));
            //insideZone = true;
            return;
        }

        if (transform.localPosition.y - threshold > SCREEN_Y / 2)
        {
            transform.position += Vector3.down * (SCREEN_Y + (threshold * 2));
            //insideZone = true;
            return;
        }

        if (transform.localPosition.y + threshold < -SCREEN_Y / 2)
        {
            transform.position += Vector3.up * (SCREEN_Y + (threshold * 2));
            //insideZone = true;
            return;
        }
    }
}
