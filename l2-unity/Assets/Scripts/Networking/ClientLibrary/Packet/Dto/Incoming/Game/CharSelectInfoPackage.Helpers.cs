using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;

public partial class CharSelectInfoPackage
{
    public double MaxExp() { return LevelServer.GetExp(_lvl + 1); }
}
