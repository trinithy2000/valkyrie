using System.Collections.Generic;
using UnityEngine;

// Util class to convert colour names to RGB strings
// Returns input if not found
public class ColorUtil
{
    public static string FromName(string name)
    {
        if (LookUp().ContainsKey(name.ToLower()))
        {
            return LookUp()[name.ToLower()];
        }
        // No match found
        return name;
    }

    public static Color ColorFromName(string name)
    {
        string colorRGB = FromName(name);
        // Check format is valid
        if ((colorRGB.Length != 7 && colorRGB.Length != 9) || (colorRGB[0] != '#'))
        {
            Game.Get().quest.log.Add(new Quest.LogEntry("Warning: Color must be in #RRGGBB format or a known name: " + name, true));
        }

        // State with white (used for alpha)
        Color colour = Color.white;
        // Hexadecimal to float convert (0x00-0xFF -> 0.0-1.0)
        colour[0] = System.Convert.ToInt32(colorRGB.Substring(1, 2), 16) / 255f;
        colour[1] = System.Convert.ToInt32(colorRGB.Substring(3, 2), 16) / 255f;
        colour[2] = System.Convert.ToInt32(colorRGB.Substring(5, 2), 16) / 255f;
        if (colorRGB.Length == 9)
        {
            colour[3] = System.Convert.ToInt32(colorRGB.Substring(7, 2), 16) / 255f;
        }

        return colour;
    }

    // Staticly defined dictionary of names to RGB strings
    // Data should match web standards
    public static Dictionary<string, string> LookUp()
    {
        Dictionary<string, string> lookUp = new Dictionary<string, string>
        {
            { "black", "#000000" },
            { "white", "#FFFFFF" },
            { "red", "#FF0000" },
            { "lime", "#00FF00" },
            { "blue", "#0000FF" },
            { "yellow", "#FFFF00" },
            { "aqua", "#00FFFF" },
            { "cyan", "#00FFFF" },
            { "magenta", "#FF00FF" },
            { "fuchsia", "#FF00FF" },
            { "silver", "#C0C0C0" },
            { "gray", "#808080" },
            { "maroon", "#800000" },
            { "olive", "#808000" },
            { "green", "#008000" },
            { "purple", "#800080" },
            { "teal", "#008080" },
            { "navy", "#000080" },
            { "transparent", "#00000000" }
        };

        return lookUp;
    }
}
