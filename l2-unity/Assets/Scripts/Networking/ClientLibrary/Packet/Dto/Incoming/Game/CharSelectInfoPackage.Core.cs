using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;

public partial class CharSelectInfoPackage
{
    public CharSelectInfoPackage()
    {
        _appearance = new PlayerAppearance();
        _paperDool = new PaperDollSelection();
        _stats = new PlayerStats();
    }
}
