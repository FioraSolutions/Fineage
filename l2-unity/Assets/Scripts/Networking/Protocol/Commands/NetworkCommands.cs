using System.Collections.Generic;
using UnityEngine;

public sealed partial class MoveToCommand : INetworkCommand
{
    public Vector3 From;
    public Vector3 To;

}

/// <summary>Fineage Net TX opcode 32. Body key 0 is the clan name.</summary>}
