using System;

public static partial class PlayerCommands
{
    public static PlayerCommandData GetById(int id)
    {
        if ((uint)id <= MAX_ID)
        {
            var data = _byId[id];
            return data != null && data.Id == id ? data : null;
        }

        return null;
    }

    public static PlayerCommandData FindByText(string text)
    {
        if (string.IsNullOrEmpty(text) || text[0] != '/')
            return null;

        for (int i = 0; i <= MAX_ID; i++)
        {
            var cmd = _byId[i];
            if (cmd != null && text.StartsWith(cmd.Command, StringComparison.Ordinal))
                return cmd;
        }

        return null;
    }
}
