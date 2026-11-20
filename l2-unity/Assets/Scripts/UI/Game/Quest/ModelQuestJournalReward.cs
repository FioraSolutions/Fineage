public sealed class ModelQuestJournalReward
{
    readonly string _name;
    readonly string _amount;
    readonly string _icon;

    public ModelQuestJournalReward(string name, string amount, string icon)
    {
        _name = name;
        _amount = amount;
        _icon = icon;
    }

    public string NameReward => _name;
    public string DecReward => _amount;
    public string Icon => _icon;
}