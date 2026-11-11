using System;
using System.Collections.Generic;

public abstract partial class PlayerAnimStateMapBase
{
    /// <summary>Prefixes checked in order; put longer ones first (e.g. atkwait_ before wait_).</summary>
    protected abstract IReadOnlyList<string> PrefixesLongestFirst { get; }




    /// <summary>
    /// Split animName into known prefix + weapon suffix, then rebuild canonical state name.
    /// </summary>
}
