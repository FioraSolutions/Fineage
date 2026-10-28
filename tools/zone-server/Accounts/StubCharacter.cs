namespace Fineage.ZoneServer.Accounts;

/// <summary>Account-local character row for Interlude stub (no DB, no inventory).</summary>
public sealed record StubCharacter(
    int ObjectId,
    string Name,
    string Account,
    int Sex,
    int Race,
    int ClassId,
    int HairStyle,
    int HairColor,
    int Face);
