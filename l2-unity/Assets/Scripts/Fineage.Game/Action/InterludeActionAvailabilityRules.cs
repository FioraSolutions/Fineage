using System;
using UnityEngine;

/// <summary>Derives <see cref="InterludeActionAvailabilityKind"/> from Interlude actionname rows (no per-id switches).</summary>
public static class InterludeActionAvailabilityRules
{
  public static bool TryResolve(
      InterludeActionNameFile.Row row,
      out InterludeActionAvailabilityKind kind,
      out int[] templateNpcIds)
  {
    kind = InterludeActionAvailabilityKind.None;
    templateNpcIds = Array.Empty<int>();

    if (row == null)
      return false;

    string desc = row.Desc ?? string.Empty;
    if (string.Equals(desc, "mountdismount", StringComparison.OrdinalIgnoreCase))
    {
      kind = InterludeActionAvailabilityKind.MountablePetSummonedOrMounted;
      return true;
    }

    if (TryTemplateNpcIds(row.Category2, out templateNpcIds))
    {
      kind = InterludeActionAvailabilityKind.SummonTemplateActive;
      return true;
    }

    if (row.Category == 4 && IsBroadSummonCommand(row.Category2))
    {
      kind = InterludeActionAvailabilityKind.PetSummoned;
      return true;
    }

    if (row.Category == 5 && IsBroadSummonCommand(row.Category2))
    {
      kind = InterludeActionAvailabilityKind.ServitorSummoned;
      return true;
    }

    if (row.Category is 0 or 1 or 2 or 3)
    {
      kind = InterludeActionAvailabilityKind.None;
      return true;
    }

    Debug.LogError(
        "[ActionAvailability] Unknown requirement for action id=" + row.Id +
        " category=" + row.Category + " desc='" + desc + "' cmd='" + row.Cmd + "'.");
    kind = InterludeActionAvailabilityKind.Unknown;
    return false;
  }

  static bool IsBroadSummonCommand(int[] category2)
  {
    if (category2 == null || category2.Length == 0)
      return true;
    if (category2.Length == 1 && category2[0] == -2)
      return true;
    return false;
  }

  static bool TryTemplateNpcIds(int[] category2, out int[] npcIds)
  {
    npcIds = Array.Empty<int>();
    if (category2 == null || category2.Length == 0)
      return false;

    int count = 0;
    for (int i = 0; i < category2.Length; i++)
    {
      if (category2[i] > 0)
        count++;
    }

    if (count == 0)
      return false;

    var exact = new int[count];
    int n = 0;
    for (int i = 0; i < category2.Length; i++)
    {
      if (category2[i] > 0)
        exact[n++] = category2[i];
    }

    npcIds = exact;
    return true;
  }
}