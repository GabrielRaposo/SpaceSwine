using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParseInputTag : MonoBehaviour
{
    [SerializeField] [TextArea(2,4)] string text;
    [SerializeField] string separator;
    [SerializeField] TMP_Text textDisplay;

    private void OnEnable() 
    {
        InputTagController.OnInputTypeChanged += ParseText;
    }

    private void OnDisable() 
    {
        InputTagController.OnInputTypeChanged -= ParseText;
    }

    void Start()
    {
        DisplayParsedText(text);
    }

    private void ParseText()
    {
        DisplayParsedText(text);
    }

    public void DisplayParsedText( string  localText )
    {
        text = localText;

        if (!textDisplay)
            textDisplay = GetComponentInChildren<TMP_Text>();

        if (!textDisplay)
            return;

        string output = string.Empty;

        // -- Método que só funciona pra um tag por frase
        /**
        foreach (string text in localTexts)
        {
            if (!text.Contains(separator))
            {
                output += text + "\n";
                continue;
            }

            int len = text.Length;
            if (len < 3) 
            {
                output += text + "\n";
                continue;
            }

            int first = text.IndexOf(separator);
            int last = text.LastIndexOf(separator);
            //Debug.Log($"first {first}, last {last}");

            string firstPart  = text.Substring (0, first);
            string middlePart = text.Substring (first + 1, last - (first + 1) );
            string lastPart   = text.Substring (last  + 1, (text.Length - 1) - last );

            middlePart = InputTagController.GetInput(middlePart);

            output += firstPart + middlePart + lastPart + "\n";
        }
        **/

        textDisplay.text = ParsedOutput( text, separator );
    }

    public static string ParsedOutput (string localText, string separator)
    {
        string output = string.Empty;

        int len = localText.Length;
        if (len < 3) 
        {
            return localText;
        }

        string subString = localText;
        while ( subString.Contains(separator) )
        {
            // -- Pega o primeiro separator
            int first = subString.IndexOf(separator);
            string firstPart  = subString.Substring (0, first);
            output += firstPart;
            
            if (first == subString.Length - 1)
                break;

            subString = subString.Substring(first + 1);
            if ( !subString.Contains(separator) )
            {
                output += subString;
                break;
            }

            // -- Pega o segundo separator
            int second = subString.IndexOf(separator);
            string secondPart = subString.Substring (0, second);

            output += InputTagController.GetInput(secondPart);

            subString = subString.Substring(second + 1);

            if (!subString.Contains(separator))
            {
                output += subString;
                break;
            }
        }

        if (output == string.Empty)
            return localText;

        return output;
    }

}
