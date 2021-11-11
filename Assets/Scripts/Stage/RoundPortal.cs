using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoundPortal : MonoBehaviour
{
    UnityAction OnContact;

    public void Setup (UnityAction OnContact)
    {
        this.OnContact = OnContact;
        gameObject.SetActive(true);
    } 

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (!collision.CompareTag("Player"))
            return;

        OnContact?.Invoke();
    }
}
