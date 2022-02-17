using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceLauncher : MonoBehaviour
{
    [SerializeField] GameObject visualComponent;

    bool interactable;

    void Start()
    {
        
    }

    private void OnTriggerEnter2D (Collider2D collision) 
    {
        if (!interactable)
            return;

        if (collision.CompareTag("Player"))
        {
            SpaceJumper spaceJumper = collision.GetComponent<SpaceJumper>();
            if (!spaceJumper)
                return;

            spaceJumper.transform.position = transform.position;
            spaceJumper.LaunchIntoDirection(Vector2.right);
            UpdateVisualState(false);

            interactable = false;
        }
    }

    private void UpdateVisualState(bool value)
    {
        visualComponent?.SetActive(value);
    }
}
