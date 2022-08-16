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

        this.direction = direction;

        if (direction == Vector2.zero)
            return;

        transform.eulerAngles = Vector2.SignedAngle(Vector2.up, direction) * Vector3.forward;
    }

    public Vector2 GetDirection()
    {
        if (direction == Vector2.zero)
        {
            float angle = (transform.eulerAngles.y == 0) ?
                transform.eulerAngles.z : 
                360 - transform.eulerAngles.z;
            //Debug.Log("angle: " + angle);

            return RaposUtil.RotateVector(Vector2.up, angle);
        }

        //Debug.Log($"x: {direction.x.ToString("0.00000")}, y: {direction.y.ToString("0.00000")}");
        return direction;
    }
}
