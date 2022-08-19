using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextFunctions
{
    public static (string output, List<TextBoxTag> tags) ParseTags (string input)
    {
        char separator = '§';
        if (!input.Contains( separator.ToString() ))
            return (input, null);

        List<TextBoxTag> tags = new List<TextBoxTag>();

        string[] subStrings = input.Split(separator);
        string output = subStrings[subStrings.Length - 1].TrimStart(); // -- os tags devem sempre estar no início do texto
        for (int i = 0; i < subStrings.Length - 1; i++)
        {
            string subString = subStrings[i].Trim().ToLower();

            if (subString.CompareTo("instant text") > -1)
            {   
                //Debug.Log("TAG: Instant Text");
                if (!tags.Contains(TextBoxTag.InstantText))
                    tags.Add(TextBoxTag.InstantText);
            }

            if (subString.CompareTo("award sound") > -1)
            {   
                //Debug.Log("TAG: Award Sound");
                if (!tags.Contains(TextBoxTag.AwardSound))
                    tags.Add(TextBoxTag.AwardSound);
            }
        }

        return (output, tags);
    }
}

public enum TextBoxTag 
{
    InstantText, AwardSound
} 
