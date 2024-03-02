using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugActivator : MonoBehaviour
{
    const int MAX_CLICKS = 30;

    [SerializeField] InputAction activationAction;
    [SerializeField] GameObject debugFunctionsObject;
    [SerializeField] AK.Wwise.Event unlockAKEvent;
    [SerializeField] float resetCooldown;

    float t;
    int clickCount = 0;

    void OnEnable()
    {
        activationAction.performed += (ctx) => 
        {
            clickCount++;
            t = resetCooldown;
        };
        activationAction.Enable();
    }

    void Start()
    {
        if (debugFunctionsObject == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!Application.isEditor)
            debugFunctionsObject.SetActive(false);
    }

    void Update()
    {
        if (clickCount < 1)
            return;
        
        t -= Time.unscaledDeltaTime;

        if (t <= 0)
        {
            clickCount = 0;
            return;
        }

        if (clickCount > MAX_CLICKS)
        {
            debugFunctionsObject.SetActive(true);
            this.Wait(2, () => 
            {
                DebugDisplay.Log("Debug Inputs Unlocked!");

                if (unlockAKEvent != null)
                    unlockAKEvent.Post(gameObject);

                enabled = false;
            });
        }
    }

    void OnDisable()
    {
        activationAction.Disable();
    }
}
