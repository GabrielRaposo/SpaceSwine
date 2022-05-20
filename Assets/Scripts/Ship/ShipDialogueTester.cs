using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipDialogueTester : MonoBehaviour
{
    [SerializeField] InputAction testInput1;
    [SerializeField] InputAction testInput2;

    [SerializeField] InputAction testInput3;
    [SerializeField] InputAction testInput4;

    ShipDialogueBox box;

    void OnEnable()
    {
        box = GetComponentInChildren<ShipDialogueBox>();

        testInput1.performed += (ctx) => 
        {
            box.SetOpenedState();
        };
        testInput2.performed += (ctx) => 
        {
            box.SetClosedState();
        };

        testInput3.performed += (ctx) => 
        {
            box.SetAllWidth(ShipDialogueBox.BASE_WIDTH);
        };
        testInput4.performed += (ctx) => 
        {
            box.SetAllWidth(ShipDialogueBox.BASE_WIDTH - 100);
        };

        testInput1.Enable();
        testInput2.Enable();
        testInput3.Enable();
        testInput4.Enable();
    }

    void OnDisable() 
    {
        testInput1.Disable();
        testInput2.Disable();
        testInput3.Disable();
        testInput4.Disable();
    }
}
