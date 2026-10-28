using System.Collections.ObjectModel;

namespace Fineage.ZoneServer.Accounts;

public enum StubCreateResult
{
    Ok,
    TooManyCharacters,
    NameAlreadyExists,
    InvalidName,
    NotHumanFighter,
}

/// <summary>In-memory characters keyed by account (process-local stub persist).</summary>
public sealed class AccountCharacterStore
{
    public const int MaxCharactersPerAccount = 7;

    readonly object _gate = new();
    readonly Dictionary<string, List<StubCharacter>> _byAccount = new(StringComparer.OrdinalIgnoreCase);
    int _nextObjectId = 1;

    public static AccountCharacterStore Shared { get; } = new();

    public ReadOnlyCollection<StubCharacter> GetCharacters(string account)
    {
        if (string.IsNullOrWhiteSpace(account))
            return ReadOnlyCollection<StubCharacter>.Empty;

        lock (_gate)
        {
            if (!_byAccount.TryGetValue(account.Trim(), out var list))
                return ReadOnlyCollection<StubCharacter>.Empty;
            return list.AsReadOnly();
        }
    }

    public StubCreateResult TryCreate(
        string account,
        string name,
        int race,
        int sex,
        int classId,
        int hairStyle,
        int hairColor,
        int face,
        out StubCharacter? created)
    {
        created = null;
        if (string.IsNullOrWhiteSpace(account))
            return StubCreateResult.InvalidName;

        account = account.Trim();
        if (!IsValidName(name))
            return StubCreateResult.InvalidName;

        if (!HumanFighterDefaults.IsHumanFighter(race, classId))
            return StubCreateResult.NotHumanFighter;

        lock (_gate)
        {
            if (!_byAccount.TryGetValue(account, out var list))
            {
                list = new List<StubCharacter>();
                _byAccount[account] = list;
            }

            if (list.Count >= MaxCharactersPerAccount)
                return StubCreateResult.TooManyCharacters;

            foreach (var c in list)
            {
                if (string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase))
                    return StubCreateResult.NameAlreadyExists;
            }

            foreach (var kv in _byAccount)
            {
                foreach (var c in kv.Value)
                {
                    if (string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase))
                        return StubCreateResult.NameAlreadyExists;
                }
            }

            int objectId = _nextObjectId++;
            var row = new StubCharacter(objectId, name, account, sex, race, classId, hairStyle, hairColor, face);
            list.Add(row);
            created = row;
            return StubCreateResult.Ok;
        }
    }

    public bool TryGetBySlot(string account, int slot, out StubCharacter? character)
    {
        character = null;
        if (string.IsNullOrWhiteSpace(account) || slot < 0)
            return false;

        lock (_gate)
        {
            if (!_byAccount.TryGetValue(account.Trim(), out var list) || slot >= list.Count)
                return false;
            character = list[slot];
            return true;
        }
    }

    static bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
        name = name.Trim();
        if (name.Length > 16)
            return false;
        foreach (char ch in name)
        {
            if (ch is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9')
                continue;
            return false;
        }

        return true;
    }
}
