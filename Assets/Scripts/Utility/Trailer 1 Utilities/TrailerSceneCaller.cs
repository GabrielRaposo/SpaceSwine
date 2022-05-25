using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class TrailerSceneCaller : MonoBehaviour
{
    [SerializeField] InputAction turnOnInput; // input para ligar
    [SerializeField] InputAction shakeInput;  // input para chamar o ScreenShake + Buyk Pulando + possíveis efeitos visuais

    [Space(10)]
    
    [SerializeField] [TextArea(minLines: 2, maxLines: 3)] List<string> shipDialogue;
    
    [Header("References")]
    [SerializeField] ShipDialogueBox shipDialogueBox;
    [SerializeField] GalaxyParallaxTester galaxyParallax;
    [SerializeField] ScreenShakeCaller screenShake;
    [SerializeField] GameObject relaxingBuyk;
    [SerializeField] GameObject startledBuyk;

    Sequence sequence;

    private void OnEnable() 
    {
        turnOnInput.performed += TurnOn;
        shakeInput.performed += Shake;

        turnOnInput.Enable();    
        shakeInput.Enable();
    }

    private void TurnOn (InputAction.CallbackContext ctx)
    {
        if (sequence != null)
        {
            sequence.Complete();
            sequence.Kill();
        }

        sequence = DOTween.Sequence();

        // -- Reseta paralaxe do fundo
        sequence.AppendCallback( () => galaxyParallax.PlaySequence() );
        sequence.AppendInterval(1.0f);

        // -- Mostra a caixa de texto
        float t1 = .5f;
        sequence.AppendCallback( () => shipDialogueBox.SetShown(true, t1) );
        sequence.AppendInterval( t1 + .5f);

        // -- Leitura dos diálogos
        foreach (string s in shipDialogue)
        {
            sequence.AppendCallback( () => shipDialogueBox.Type(s) );

            float t2 = 3.0f;
            t2 += s.Length * .015f;
            sequence.AppendInterval(t2);
        }

        sequence.AppendCallback( () => shipDialogueBox.Type(" ") );
        sequence.AppendCallback( () => shipDialogueBox.SetShown(false) );
    }

    private void Shake (InputAction.CallbackContext ctx)
    {
        if (sequence != null)
            sequence.Kill();

        screenShake.CallScreenshake();

        relaxingBuyk.SetActive(false);
        startledBuyk.SetActive(true);

        shipDialogueBox.Type("<color=red><b>-- AMEAÇA DETECTADA --", delay: .01f, instantText: true);
        shipDialogueBox.transform.DOPunchScale(Vector3.one * .8f, duration: .3f);
        shipDialogueBox.transform.DOLocalRotate(Vector3.forward * -3, duration: .1f);

        RaposUtil.WaitSeconds(this, 7, GameManager.ResetScene );
    }

    private void OnDisable() 
    {
        turnOnInput.Disable();    
        shakeInput.Disable();
    }
}
