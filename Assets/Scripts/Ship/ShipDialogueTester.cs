using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipDialogueTester : MonoBehaviour
{
    [SerializeField] ShipDialogueBox box;

    [SerializeField] InputAction testInput1;
    [SerializeField] InputAction testInput2;

    [SerializeField] InputAction testInput3;
    [SerializeField] InputAction testInput4;

    void OnEnable()
    {
        if (!box)
            box = GetComponentInChildren<ShipDialogueBox>();

        testInput1.performed += (ctx) => 
        {
            if (!box.gameObject.activeSelf)
                return;
            box.Type("Oi, eu meu chamo [P.I.G.]\nEu serei a sua companheira nesta missão.");
        };
        testInput2.performed += (ctx) => 
        {
            if (!box.gameObject.activeSelf)
                return;
            box.Type("É de extrema importância que a minha comunicação com você esteja perfeitamente funcional.");
        };
        testInput3.performed += (ctx) => 
        {
            if (!box.gameObject.activeSelf)
                return;
            //box.Type("<color=red><size=20><anim=fullshake>PERIGO</anim>", delay: .01f, instantText: true);
            box.Type("Linha 1\nLinha 2\nLinha 3\nLinha 4");
        };
        testInput4.performed += (ctx) => 
        {
            box.gameObject.SetActive(!box.gameObject.activeSelf);
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
