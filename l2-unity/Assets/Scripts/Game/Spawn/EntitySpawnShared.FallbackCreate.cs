using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class EntitySpawnShared
{




    /// <summary>
    /// Monster rigs the art pack does not ship. Elven Village is the loud case: Dryads, Lireins and
    /// Spore Fungus have no prefab, so the server kept them alive while the client dropped every row
    /// from the knownlist ("not all the mobs around Elven Village"). Substituting a shipped rig by
    /// silhouette keeps the mob visible, targetable and killable; the alternative — hiding it — is
    /// what made the area look empty.
    /// </summary>
}
