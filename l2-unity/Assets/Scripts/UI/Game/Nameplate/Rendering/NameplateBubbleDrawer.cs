using System.Collections.Generic;
using UnityEngine;

public sealed partial class NameplateBubbleDrawer
{
    private readonly L2NameplateScreenBatch _batchNormal = new L2NameplateScreenBatch();
    private readonly L2NameplateScreenBatch _batchTarget = new L2NameplateScreenBatch();
    private readonly L2NameplateScreenBatch _batchAttack = new L2NameplateScreenBatch();

    private Texture2D _texNormal;
    private Texture2D _texTarget;
    private Texture2D _texAttack;
    private bool _enabled = true;

    public bool Enabled
    {
        get => _enabled;
        set => _enabled = value;
    }




}
