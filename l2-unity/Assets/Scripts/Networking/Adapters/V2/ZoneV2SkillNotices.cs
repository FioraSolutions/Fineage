using System;

/// <summary>User-facing lines for Backend <c>SkillProtocol</c> error codes on Zone V2.</summary>
public static class ZoneV2SkillNotices
{
    public static bool TryLine(string code, string message, out string line)
    {
        line = null;
        if (string.IsNullOrEmpty(code) || !code.StartsWith("skill_", StringComparison.Ordinal))
            return false;
        line = Notice(code, message);
        return true;
    }

    public static string Notice(string code, string message)
    {
        switch (code)
        {
            case "skill_unknown":
                return "Unknown skill.";
            case "skill_passive":
                return "Passive skills cannot be used.";
            case "skill_reuse":
                return "Skill is not ready yet.";
            case "skill_mp":
                return "Not enough MP.";
            case "skill_target":
                return "Invalid target.";
            case "skill_range":
                return "Target is out of range.";
            case "skill_dead":
                return "You cannot use skills while dead.";
        }

        if (!string.IsNullOrWhiteSpace(message))
            return message.Trim();
        return code ?? "";
    }
}
