using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

public partial class CharacterSelector
{

    /// <summary>
    /// Drop select-row visuals while still in lobby (roster refresh). Uses normal Destroy.
    /// World enter relies on Menu unload, not this.
    /// </summary>


    /// <summary>
    /// After Menu unload the scene objects are gone; only drop local refs.
    /// </summary>







    /// <summary>
    /// Maps login paperdoll (or legacy WeaponItemId) onto CharSelect appearance.
    /// CreatePawnInterlude then SyncEquippedArmor + EquipWeapon from these fields.
    /// </summary>




}
