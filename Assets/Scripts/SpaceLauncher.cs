using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceLauncher : MonoBehaviour
{
    [SerializeField] GameObject visualComponent;

    bool interactable = true;

    void Start()
    {

    }

    private void OnTriggerStay2D (Collider2D collision) 
    {
        if (!interactable)
            return;

        Vector2 direction = transform.position - collision.transform.position;
        float angle = Vector2.SignedAngle(Vector2.up, direction);
        Vector2 anchoredDirection = RaposUtil.RotateVector(direction, angle);

        //float distance = Vector2.Distance(collision.transform.position, transform.position);
        //Debug.Log("distance: " + distance);
        if (anchoredDirection.y > .15f)
            return;

        if (collision.CompareTag("Player"))
        {
            SpaceJumper spaceJumper = collision.GetComponent<SpaceJumper>();
            if (!spaceJumper)
                return;

            spaceJumper.transform.position = transform.position;
            spaceJumper.RedirectIntoDirection(Vector2.right);
            UpdateVisualState(false);

            interactable = false;
            Debug.Log("Called event!!!!");
        }
    }

    private void UpdateVisualState(bool value)
    {
        visualComponent?.SetActive(value);
    }
}
