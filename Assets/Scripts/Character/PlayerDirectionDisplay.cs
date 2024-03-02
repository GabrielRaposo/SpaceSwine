﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirectionDisplay : MonoBehaviour
{
    public Vector2 direction;

    [Header("Raycasted Line")]
    [SerializeField] bool useRaycastedLine;
    [SerializeField] SpriteRenderer raycastedLine;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Color hiddenColor;
    [SerializeField] Color shownColor;
    [SerializeField] float fadeCountMin;
    [SerializeField] float fadeCountMax;
    [SerializeField] float fadeOutRatio;
    [SerializeField] float fadeInRatio;

    float fadeCount;
    float previousAngle;
    float previousParentAngle; // -- Para planetas e vigas que giram sozinhos
    bool isLanded;
    bool blockfade;

    Transform parent;    
    LocalGameplayState localGameplayState;
    SpriteRenderer[] spriteRendereres;
    

    private void Awake() 
    {
        localGameplayState = GetComponentInParent<LocalGameplayState>();
        spriteRendereres = GetComponentsInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        previousAngle = transform.localEulerAngles.z;
        HideLine();
    }

    private void Start() 
    {
        Dettach();
        SetVisibility (true);

        if (!useRaycastedLine && raycastedLine)
            raycastedLine.enabled = false;
    }

    private void Dettach()
    {
        parent = transform.parent;
        transform.SetParent(null);
    }

    bool AngleChangedTooMuch => Mathf.Abs (previousAngle - transform.localEulerAngles.z) > .01f;
    bool ParentAngleChangedTooMuch(Transform parentTransform) => Mathf.Abs (previousParentAngle - parentTransform.eulerAngles.z) > .01f;

    private void Update() 
    {
        if (parent == null)
            return;

        GravitationalBody gBody = parent.GetComponentInParent<GravitationalBody>();
        isLanded = gBody;

        transform.position = parent.position;
        SetVisibility(parent.gameObject.activeInHierarchy);

        if (!useRaycastedLine || !raycastedLine)
            return;
        
        // -- Raycast Length
        { 
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 15f, groundLayer); 
            float lineLength = hit ? (Vector2.Distance(parent.position, hit.point) - 0.8125f) : 15; 
            raycastedLine.size = new Vector2 (lineLength, 0.0625f);
        }

        // -- Raycast Color
        {
            if (blockfade)
                return;

            if ( !AngleChangedTooMuch || (isLanded && gBody && ParentAngleChangedTooMuch(gBody.transform)) )
            {
                fadeCount += (Time.deltaTime * fadeInRatio);
                if (fadeCount > fadeCountMax)
                    fadeCount = fadeCountMax;
            }
            else
            {
                fadeCount -= (Time.deltaTime * fadeOutRatio);
                if (fadeCount < fadeCountMin)
                    fadeCount = fadeCountMin;
            }

            Color c = Color.Lerp(hiddenColor, shownColor, Mathf.Clamp01 (fadeCount));
            raycastedLine.color = c;
        }

        previousAngle = transform.localEulerAngles.z;
        previousParentAngle = (isLanded ? gBody.transform.eulerAngles.z : 0 );
    }

    public void SetVisibility (bool value)
    {
        if (!localGameplayState || localGameplayState.state == GameplayState.Exploration)
        {
            gameObject.SetActive(false);
            return;
        }
        
        if (spriteRendereres.Length < 1)
            return;

        foreach (var sprite in spriteRendereres)
            sprite.enabled = value;
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

        if (this.direction != direction)
            HideLine();
        
        this.direction = direction;

        if (direction == Vector2.zero)
            return;

        transform.eulerAngles = Vector2.SignedAngle(Vector2.up, direction) * Vector3.forward;
    }

    public void HideLine(bool blockfade = false)
    {
        if (!raycastedLine)
            return;

        fadeCount = fadeCountMin;
        raycastedLine.color = hiddenColor;

        this.blockfade = blockfade;
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
