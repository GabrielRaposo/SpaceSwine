using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubNPCSetter : MonoBehaviour
{
    [SerializeField] InteractableNPC mainNPC;
    [SerializeField] InteractableNPC subNPC;  

    void Start()
    {
        if (!mainNPC || !subNPC)
            return;

        subNPC.SetInteraction(false);
        //subNPC.gameObject.SetActive(false);
        StartCoroutine(DelayDeactivation());

        subNPC.GetComponentInChildren<Animator>().SetTrigger("StartUnmasked");
    }

    IEnumerator DelayDeactivation()
    {
        yield return new WaitForSeconds(1f);
        subNPC.gameObject.SetActive(false);
    }

    public void SetSubNPC()
    {
        if (!mainNPC || !subNPC)
            return;

        mainNPC.SetInteraction(false);
        mainNPC.gameObject.SetActive(false);

        subNPC.SetInteraction(true);
        subNPC.gameObject.SetActive(true);
    }
}
