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
            box.ToggleVerticalTransition();
        };
        testInput2.performed += (ctx) => 
        {
            box.ToggleVerticalTransition(duration: .1f);
        };

        testInput3.performed += (ctx) => 
        {
            box.Type("Oi, eu meu chamo  <i>P. I. G.</i>\nEu serei a sua companheira nesta missão.");
            //box.SetAllWidth(ShipDialogueBox.BASE_WIDTH);
        };
        testInput4.performed += (ctx) => 
        {
            box.ToggleVerticalTransition();
            box.Type("É de extrema importância que a minha comunicação com você esteja perfeitamente funcional.", delay: .5f);
            //box.SetAllWidth(ShipDialogueBox.BASE_WIDTH - 100);
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
