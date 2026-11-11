public partial class ToolTipSkill
{
    private int ParceCellId(string name)
    {
        string strId = name.Replace("imgbox", "");
        return Int32.Parse(strId);
    }

    private int ParcePassiveCellId(string name)
    {
        string strId = name.Replace("pasbox", "");
        return Int32.Parse(strId);
    }
}
