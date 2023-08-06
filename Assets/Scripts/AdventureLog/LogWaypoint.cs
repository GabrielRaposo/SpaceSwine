using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LogWaypoint : MonoBehaviour
{
    [SerializeField] float hideRadius;
    [SerializeField] SpriteRenderer mainRenderer;
    [SerializeField] List<AdventureLogScriptableObject> logs;

    PlayerCharacter playerCharacter;
    LogWaypointArrow UIArrow;

    void Start()
    {
        StartCoroutine ( WaitForPlayerReference ( Initiate ) );
    }

    private IEnumerator WaitForPlayerReference (UnityAction action)
    {
        yield return new WaitWhile ( () => PlayerCharacter.Instance == null );
        
        playerCharacter = PlayerCharacter.Instance;
        UIArrow = LogWaypointArrow.Instance;

        action.Invoke();
    }

    private void Initiate()
    {
        if (!playerCharacter || !UIArrow)
        {
            gameObject.SetActive(false);
            return;
        }

        UpdateVisibility();
    }

    void Update()
    {
        if (!playerCharacter)
            return;

        UpdateVisibility();

        UIArrow.UpdateDirection( (transform.position - playerCharacter.transform.position).normalized );
    }

    private void UpdateVisibility()
    {
        mainRenderer.enabled = Vector2.Distance(transform.position, playerCharacter.transform.position) > hideRadius;
    }
}
