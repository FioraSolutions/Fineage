using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public partial class SceneLoader
{




    /// <summary>
    /// Game.unity ships an inactive <c>Prefabs</c> catalog (faces/hair/npc meshes parked
    /// near the school). It must stay off — enabling all Game roots would show them in-world.
    /// </summary>


    /// <summary>
    /// Destroy lobby presentation, then unload Menu + l2_lobby and wait until gone.
    /// </summary>


    /// <summary>Enable gameplay follow-cam; it must own Camera.main after lobby is gone.</summary>


}
