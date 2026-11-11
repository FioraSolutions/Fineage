using System;
using System.Collections.Generic;
using System.Linq;

public partial class DatUtils
{
    public static List<List<string>> ParseMultiArray(string value)
    {
        return SplitJSONMultiArr(value);
    }

    public static string ParseIcon(string value) {
        return SplitJSON(value)[0];
    }

    public static string[] ParseOtherIcon(string value)
    {
        return SplitJSON(value);
    }

    public static string[] SplitJSON(string value) {
        return value.Replace("{", string.Empty).Replace("}", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Split(";");
    }

    public static List<List<string>> SplitJSONMultiArr(string value)
    {
        // Remove outermost curly braces
        string inner = value.Trim().Substring(1, value.Length - 2);

        // Split into top-level arrays (separated by };{)
        string[] topLevelArrays = inner.Split(new[] { "};{" }, StringSplitOptions.None);

        // Create a list to hold our results
        var result = new List<List<string>>();

        foreach (var array in topLevelArrays)
        {
            // Process each array
            string cleanedArray = array.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
            string[] elements = cleanedArray.Split(';');

            // Add the elements as a new list to our result
            result.Add(elements.ToList());
        }

        return result;
    }
}
