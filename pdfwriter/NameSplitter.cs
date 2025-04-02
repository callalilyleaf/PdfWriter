using System;
using System.Text;
using System.Text.RegularExpressions;

public class NameSplitter
{
    public static string AddSpaceBeforeLastName(string fullName)
    {
        if (string.IsNullOrEmpty(fullName))
        {
            return fullName;
        }

        StringBuilder result = new StringBuilder();
        result.Append(fullName[0]); // Always add the first character

        bool lastNameStarted = false;
        for (int i = 1; i < fullName.Length; i++)
        {
            if (!lastNameStarted && char.IsUpper(fullName[i]) && i > 0 && char.IsLower(fullName[i - 1]))
            {
                // Current char is upper, previous is lower, not the first char
                result.Append(" ");
                lastNameStarted = true;
            }
            else if (!lastNameStarted && fullName.StartsWith("Mc", StringComparison.OrdinalIgnoreCase) && i == 2 && char.IsUpper(fullName[i]))
            {
                // Starts with "Mc" (or "mc"), 3rd char is upper
                result.Append(" ");
                lastNameStarted = true;
            }
            else if (!lastNameStarted && fullName.StartsWith("Mac", StringComparison.OrdinalIgnoreCase) && i == 3 && char.IsUpper(fullName[i]))
            {
                // Starts with "Mac" (or "mac"), 4th char is upper
                result.Append(" ");
                lastNameStarted = true;
            }

            result.Append(fullName[i]);
        }

        return result.ToString().Trim();
    }

    public static void Test()
    {
        string[] names = {
            "JohnSmith",
            "JaneDoe",
            "RobertMcDonald",
            "MaryOMalley",
            "DavidDeVries",
            "AnnaVanDerLinde",
            "PeterMcLeod",
            "JohnF",
            "Lee",
            "JohnASmith"
        };

        foreach (string name in names)
        {
            Console.WriteLine($"Original: '{name}', Modified: '{AddSpaceBeforeLastName(name)}'");
        }
    }
}