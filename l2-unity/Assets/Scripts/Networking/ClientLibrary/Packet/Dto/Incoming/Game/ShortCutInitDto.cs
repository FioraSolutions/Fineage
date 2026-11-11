using System.Collections.Generic;

public partial class ShortCutInitDto : IWireDto
{
    private List<Shortcut> _shortcut;

    public List<Shortcut> ShortCuts { get => _shortcut; }

    //L2j Enum type
    //NONE,
    //ITEM,
    //SKILL,
    //ACTION,
    //MACRO,
    //RECIPE,



}
