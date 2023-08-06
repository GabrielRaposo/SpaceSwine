using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogWaypointArrow : MonoBehaviour
{
    [SerializeField] RectTransform anchorRT;
    [SerializeField] RectTransform imageRT;

    public static LogWaypointArrow Instance;

    private void Awake() 
    {
        if (Instance != null)
        {
            gameObject.SetActive(false);
            return;
        }

        Instance = this;
    }

    private void Start() 
    {
        //SetVisibility (false);
    }

    public void SetVisibility (bool value)
    { 
        anchorRT.gameObject.SetActive(value);
    }

    const float EDGE_ANGLE1 = 57.6f;
    const float EDGE_ANGLE2 = 147.6f;
    const float EDGE_ANGLE3 = 237.6f;
    const float EDGE_ANGLE4 = 302.4f;

    const float HALF_WIDTH = 775;
    const float HALF_HEIGHT = 425;

    public void UpdateDirection (Vector2 direction)
    {
        float angle = Vector2.SignedAngle(Vector2.up, direction);

        imageRT.eulerAngles = Vector3.forward * (angle + 90);

        if (angle < 0)
            angle += 360;

        //Debug.Log("angle: " + angle);
        Vector2 minPos = Vector2.zero;
        Vector2 maxPos = Vector2.zero;
        float lValue = .5f;
        float offset = 10f;

        // -- North
        if (angle < EDGE_ANGLE1 || angle > EDGE_ANGLE4)
        {
            anchorRT.anchorMin = anchorRT.anchorMax = anchorRT.pivot = new Vector2(.5f, 1.0f);
            minPos = Vector2.right * HALF_WIDTH;
            maxPos = Vector2.right * -HALF_WIDTH;
            if (angle < 57.6f)
            {
                lValue = Mathg.Remap (angle, 0f, EDGE_ANGLE1, 0.5f, 1.0f);
            }
            else
            {
                lValue = Mathg.Remap (angle, EDGE_ANGLE4, 360f, 0f, 0.5f);
            }
        }
        // -- West
        else if (angle < EDGE_ANGLE2)
        {
            anchorRT.anchorMin = anchorRT.anchorMax = anchorRT.pivot = new Vector2(0f, .5f); 
            minPos = Vector2.up * HALF_HEIGHT;
            maxPos = Vector2.up * -HALF_HEIGHT;

            lValue = Mathg.Remap (angle, EDGE_ANGLE1, EDGE_ANGLE2, 0f, 1f);
        }
        // -- South
        else if (angle < EDGE_ANGLE3)
        {
            anchorRT.anchorMin = anchorRT.anchorMax = anchorRT.pivot = new Vector2(.5f, 0f);
            minPos = Vector2.right * -HALF_WIDTH;
            maxPos = Vector2.right * HALF_WIDTH;

            lValue = Mathg.Remap (angle, EDGE_ANGLE2, EDGE_ANGLE3, 0f, 1f);
        }
        // -- East
        else 
        {
            anchorRT.anchorMin = anchorRT.anchorMax = anchorRT.pivot = new Vector2(1.0f, .5f);
            minPos = Vector2.up * -HALF_HEIGHT;
            maxPos = Vector2.up * HALF_HEIGHT;

            lValue = Mathg.Remap (angle, EDGE_ANGLE3, EDGE_ANGLE4, 0f, 1f);
        }

        Debug.Log("lValue: " + lValue);
        anchorRT.anchoredPosition = Vector2.Lerp (minPos, maxPos, lValue);
    }
}