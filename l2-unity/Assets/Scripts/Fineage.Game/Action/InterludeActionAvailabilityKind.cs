/// <summary>
/// When an Interlude ActionWnd row is shown/enabled (parity with actionname + PetInfo / mount wire).
/// </summary>
public enum InterludeActionAvailabilityKind
{
  None = 0,
  /// <summary>Category 4 standalone pet commands (pethold, petattack, …).</summary>
  PetSummoned = 1,
  /// <summary>Category 5 standalone servitor commands (summonhold, unsummon, summonmove, …).</summary>
  ServitorSummoned = 2,
  /// <summary><c>desc=mountdismount</c> (action 38): mountable pet out or player mounted (strider / wyvern).</summary>
  MountablePetSummonedOrMounted = 3,
  /// <summary>Row bound to summon NPC template ids in <c>category2</c> (pet skill actions, siege summon skills).</summary>
  SummonTemplateActive = 4,
  /// <summary>Parsed catalog row has no rule — must not be offered silently.</summary>
  Unknown = 99,
}