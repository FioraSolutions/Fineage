using Fineage.ZoneServer.Combat;
using MessagePack;

namespace Fineage.ZoneServer.Wire;

[MessagePackObject]
public sealed class HelloMsg
{
    [Key(0)] public int ProtocolVersion { get; set; } = 1;
    [Key(1)] public string ClientBuild { get; set; } = "";
}

[MessagePackObject]
public sealed class HelloAckMsg
{
    [Key(0)] public int ProtocolVersion { get; set; } = 1;
    [Key(1)] public string ServerName { get; set; } = "FineageZone";
}

[MessagePackObject]
public sealed class AuthMsg
{
    [Key(0)] public string Account { get; set; } = "";
    [Key(1)] public string Password { get; set; } = "";
}

[MessagePackObject]
public sealed class AuthOkMsg
{
    [Key(0)] public int SessionId { get; set; }
    [Key(1)] public string CharacterName { get; set; } = "Test";
}

[MessagePackObject]
public sealed class EnterWorldMsg
{
    [Key(0)] public int CharacterSlot { get; set; }
}

/// <summary>Maps to client DomainPlayerSpawned (+ spawn pose).</summary>
[MessagePackObject]
public sealed class PlayerSpawnedMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public int X { get; set; }
    [Key(3)] public int Y { get; set; }
    [Key(4)] public int Z { get; set; }
    [Key(5)] public float RunSpeed { get; set; } = 120f;
    [Key(6)] public float WalkSpeed { get; set; } = 80f;
    [Key(7)] public bool Running { get; set; } = true;
}

/// <summary>Maps to client DomainMoveTo (L2 world units).</summary>
[MessagePackObject]
public sealed class MoveToMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int DestX { get; set; }
    [Key(2)] public int DestY { get; set; }
    [Key(3)] public int DestZ { get; set; }
    [Key(4)] public int OriginX { get; set; }
    [Key(5)] public int OriginY { get; set; }
    [Key(6)] public int OriginZ { get; set; }
}

[MessagePackObject]
public sealed class ErrorMsg
{
    [Key(0)] public string Code { get; set; } = "";
    [Key(1)] public string Message { get; set; } = "";
}

/// <summary>Thin v0.4 presence — maps to client CharInfo / DomainKnownlistAdd.</summary>
[MessagePackObject]
public sealed class CharInfoMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public int X { get; set; }
    [Key(3)] public int Y { get; set; }
    [Key(4)] public int Z { get; set; }
    [Key(5)] public int ClassId { get; set; }
}

/// <summary>Thin v0.4 presence — maps to client NpcInfo.</summary>
[MessagePackObject]
public sealed class NpcInfoMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int NpcTemplateId { get; set; }
    [Key(2)] public int X { get; set; }
    [Key(3)] public int Y { get; set; }
    [Key(4)] public int Z { get; set; }
}

[MessagePackObject]
public sealed class DeleteObjectMsg
{
    [Key(0)] public int ObjectId { get; set; }
}

[MessagePackObject]
public sealed class SelectTargetMsg
{
    [Key(0)] public int TargetObjectId { get; set; }
}

/// <summary>Interlude AttackRequest — uses current target when body empty.</summary>
[MessagePackObject]
public sealed class AttackRequestMsg
{
    [Key(0)] public int TargetObjectId { get; set; }
}

/// <summary>Maps to client MyTargetSelected.</summary>
[MessagePackObject]
public sealed class MyTargetSelectedMsg
{
    [Key(0)] public int ObjectId { get; set; }
    /// <summary>Interlude color hint (e.g. 11 = hostile red).</summary>
    [Key(1)] public int ColorHint { get; set; } = InterludeStatus.TargetColorHostile;
}

/// <summary>Thin Attack — enough for melee hit visual + damage number.</summary>
[MessagePackObject]
public sealed class AttackMsg
{
    [Key(0)] public int AttackerObjectId { get; set; }
    [Key(1)] public int TargetObjectId { get; set; }
    [Key(2)] public int Damage { get; set; }
    [Key(3)] public byte Flags { get; set; }
    [Key(4)] public int AttackerX { get; set; }
    [Key(5)] public int AttackerY { get; set; }
    [Key(6)] public int AttackerZ { get; set; }
    [Key(7)] public int TargetX { get; set; }
    [Key(8)] public int TargetY { get; set; }
    [Key(9)] public int TargetZ { get; set; }
}

[MessagePackObject]
public sealed class StatusUpdateMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int[] AttributeIds { get; set; } = Array.Empty<int>();
    [Key(2)] public int[] Values { get; set; } = Array.Empty<int>();
}
