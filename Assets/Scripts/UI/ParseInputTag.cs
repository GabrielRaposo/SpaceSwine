using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParseInputTag : MonoBehaviour
{
    [SerializeField] [TextArea(2,4)] List<string> texts;
    [SerializeField] string separator;
    [SerializeField] TMP_Text textDisplay;

    private void OnEnable() 
    {
        InputTagController.OnInputTypeChanged += ParseTexts;
    }

    void Start()
    {
        ParseTexts();
    }

    private void OnDisable() 
    {
        InputTagController.OnInputTypeChanged -= ParseTexts;
    }

    private void ParseTexts()
    {
        ParseList(texts);
    }

    public void ParseList(List<string> localTexts)
    {
        texts = localTexts;

        if (!textDisplay)
            textDisplay = GetComponentInChildren<TMP_Text>();

        if (!textDisplay)
            return;

        if (texts.Count < 1)
            return;

        string output = string.Empty;
        foreach (string text in localTexts)
        {
            if (!text.Contains(separator))
                return;

            int len = text.Length;
            if (len < 3)
                return;

            int first = text.IndexOf(separator);
            int last = text.LastIndexOf(separator);
            //Debug.Log($"first {first}, last {last}");

            string firstPart  = text.Substring (0, first);
            string middlePart = text.Substring (first + 1, last - (first + 1) );
            string lastPart   = text.Substring (last  + 1, (text.Length - 1) - last );
        
            middlePart = InputTagController.GetInput(middlePart);

            output += firstPart + middlePart + lastPart + "\n";
        }

        textDisplay.text = output;
    }

}
