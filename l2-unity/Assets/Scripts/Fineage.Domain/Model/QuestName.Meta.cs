using System.Collections.Generic;

public partial class QuestName
{
    private readonly List<QuestStepItemCount> _requiredItems = new();
    private int _questWorldX;
    private int _questWorldY;
    private int _questWorldZ;

    public int QuestWorldX { get => _questWorldX; set => _questWorldX = value; }
    public int QuestWorldY { get => _questWorldY; set => _questWorldY = value; }
    public int QuestWorldZ { get => _questWorldZ; set => _questWorldZ = value; }

    public bool HasWorldTarget => QuestWorldX != 0 || QuestWorldY != 0;

    public IReadOnlyList<QuestStepItemCount> RequiredItemCounts => _requiredItems;

    internal void AddRequiredItem(int itemId, int count)
    {
        if (itemId > 0 && count > 0)
            _requiredItems.Add(new QuestStepItemCount(itemId, count));
    }
}