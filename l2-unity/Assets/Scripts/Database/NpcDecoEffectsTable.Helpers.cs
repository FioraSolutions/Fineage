using System;

public partial class NpcDecoEffectsTable
{
    public bool TryGet(int npcId, out NpcDecoEffect deco)
    {
        return _byNpcId.TryGetValue(npcId, out deco);
    }

    static List<string> ParseCsvLine(string line)
    {
        List<string> cols = new List<string>(5);
        StringBuilder current = new StringBuilder();
        bool inQuotes = false;
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
                continue;
            }

            if (c == ',' && !inQuotes)
            {
                cols.Add(current.ToString());
                current.Length = 0;
                continue;
            }

            current.Append(c);
        }

        cols.Add(current.ToString());
        return cols;
    }
}
