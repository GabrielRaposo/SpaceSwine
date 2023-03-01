using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParseInputTag : LocalizedText
{
    public string separator;

    protected override void OnEnable() 
    {
        base.OnEnable();
        InputTagController.OnInputTypeChanged += SetText;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        InputTagController.OnInputTypeChanged -= SetText;
    }

    public override void SetText()
    {
        base.SetText();
        textMesh.text = ParsedOutput(textMesh.text, separator);
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
