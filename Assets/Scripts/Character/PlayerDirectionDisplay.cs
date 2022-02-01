using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirectionDisplay : MonoBehaviour
{
    public Vector2 direction;

    private void Start() 
    {
        LocalGameplayState localGameplayState = GetComponentInParent<LocalGameplayState>();    
        if (!localGameplayState)
            return;

        SetVisibility (localGameplayState.state == GameplayState.Danger);
    }

    private void SetVisibility(bool value)
    {
        gameObject.SetActive(value);
    }

    public void UpdateDirection (bool aiming, Vector2 direction)
    {
        if (!aiming)
        {
            transform.localEulerAngles = Vector3.zero; 
            return;
        }

        if (direction == Vector2.zero)
            return;

        transform.eulerAngles = Vector2.SignedAngle(Vector2.up, direction) * Vector3.forward;
        this.direction = direction;
    }
}
