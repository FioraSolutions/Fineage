public partial class QuestName
{
    public string GetLevelRange()
    {
        if (_lvl_max == 0)
        {
            return $"{_lvl_min}";
        }
        return $"{_lvl_min} ~ {_lvl_max}";
    }

    public string GetRepetable()
    {
        //3 not repeat
        //2  repaet
        //0 - unk

        if (_repeat == 3)
        {
            return "1";
        }else if (_repeat < 3)
        {
            return "repeat";
        }

        return "1";
    }
}
