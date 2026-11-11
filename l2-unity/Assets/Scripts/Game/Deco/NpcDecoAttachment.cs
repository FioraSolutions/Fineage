using UnityEngine;

public static partial class NpcDecoAttachment
{
    static readonly string[] BoneHints =
    {
        "r_bone1",
        "r_bone2",
        "r_bone3"
    };

    static readonly IEffectAttachmentResolver Resolver = new DefaultEffectAttachmentResolver();






}
