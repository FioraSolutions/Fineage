using System.Text.RegularExpressions;

public partial class QuestName
{
    public string GetSource()
    {
        if(!string.IsNullOrEmpty(source)) return source;

        if (string.IsNullOrEmpty(_entity_name))
            return string.Empty;

        string result = _entity_name;

        foreach (var title in _titlesToRemove)
        {
            result = result.Replace(title, "").Trim();
        }

        source = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " ").Trim();

        return source;
    }
}
