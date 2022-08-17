using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirectionDisplay : MonoBehaviour
{
    public Vector2 direction;

    Transform parent;    
    LocalGameplayState localGameplayState;
    SpriteRenderer spriteRenderer;

    private void Awake() 
    {
        localGameplayState = GetComponentInParent<LocalGameplayState>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start() 
    {
        Dettach();
        SetVisibility (true);
    }

    private void Dettach()
    {
        parent = transform.parent;
        transform.SetParent(null);
    }

    private void Update() 
    {
        if (parent == null)
            return;

        transform.position = parent.position;
        SetVisibility(parent.gameObject.activeInHierarchy);
    }

    public void SetVisibility (bool value)
    {
        if (!localGameplayState || localGameplayState.state == GameplayState.Exploration)
        {
            gameObject.SetActive(false);
            return;
        }

        spriteRenderer.enabled = value;
    }

    public void UpdateDirection (bool aiming, Vector2 direction)
    {
        if (!aiming)
        {
            Vector3 angle = Vector3.zero;
            if (parent)
            {
                //Debug.Log("previousParent: " + previousParent);
                angle = parent.eulerAngles;
            }
            transform.eulerAngles = angle; 
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
