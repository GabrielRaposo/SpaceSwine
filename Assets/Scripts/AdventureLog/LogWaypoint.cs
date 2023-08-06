using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LogWaypoint : MonoBehaviour
{
    [SerializeField] float hideRadius;
    [SerializeField] SpriteRenderer mainRenderer;
    [SerializeField] List<AdventureLogScriptableObject> logs;

    bool active;

    LogWaypointArrow UIArrow;
    Transform cameraTransform;

    void Start()
    {
        if (logs.Count < 1)
        {
            gameObject.SetActive(false);
            return;
        }
        
        cameraTransform = Camera.main.transform;
        UIArrow = LogWaypointArrow.Instance;

        if (!cameraTransform || !UIArrow)
        {
            gameObject.SetActive(false);
            return;
        }

        UpdateVisibility();
    }

    void Update()
    {
        active = false;
        foreach (AdventureLogScriptableObject log in logs)
        {
            if (AdventureLogDisplay.UpdatedList.Contains(log))
            {
                active = true;
                break;
            }
        }

        if (!active)
        {   
            UpdateVisibility();
            //UIArrow.SetVisibility(false);
            return;
        }

        UpdateVisibility();

        if (!mainRenderer.isVisible && NotOnRange)
        {
            UIArrow.UpdateDirection( (transform.position - cameraTransform.transform.position).normalized );
            UIArrow.SetVisibility(true);
            return;
        }
        UIArrow.SetVisibility(false);
    }

    private void UpdateVisibility()
    {
        if (!active)
        {
            mainRenderer.enabled = false;
            return;
        }

        mainRenderer.enabled = NotOnRange;
    }

    private bool NotOnRange => Vector2.Distance(transform.position, cameraTransform.transform.position) > hideRadius;

    private void OnDrawGizmos() 
    {
        if (!cameraTransform)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay((Vector2) cameraTransform.position, (Vector2) (transform.position - cameraTransform.position) * 5);
    }
}