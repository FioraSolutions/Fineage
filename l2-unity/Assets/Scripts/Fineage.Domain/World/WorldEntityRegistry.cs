using System;
using System.Collections.Generic;

/// <summary>
/// Object-id → entity maps for World (all / players / npcs).
/// Keeps dictionary ownership out of the MonoBehaviour façade (A05).
/// </summary>
public sealed class WorldEntityRegistry<T> where T : class
{
    readonly Dictionary<int, T> _all = new Dictionary<int, T>();
    readonly Dictionary<int, T> _players = new Dictionary<int, T>();
    readonly Dictionary<int, T> _npcs = new Dictionary<int, T>();

    public bool Contains(int id) => _all.ContainsKey(id);

    public bool ContainsNpc(int id) => _npcs.ContainsKey(id);

    public bool ContainsPlayer(int id) => _players.ContainsKey(id);

    public bool TryGet(int id, out T entity) => _all.TryGetValue(id, out entity);

    public T GetOrNull(int id)
    {
        T entity;
        return _all.TryGetValue(id, out entity) ? entity : null;
    }

    public bool TryGetPlayer(int id, out T entity) => _players.TryGetValue(id, out entity);

    public bool TryGetNpc(int id, out T entity) => _npcs.TryGetValue(id, out entity);

    public void RegisterPlayer(int id, T entity)
    {
        _players[id] = entity;
        _all[id] = entity;
    }

    /// <summary>
    /// Register a remote/local user under players+all when absent.
    /// Returns false when <paramref name="id"/> is already present (caller should notify waiters with existing).
    /// </summary>
    public bool TryRegisterUser(int id, T entity)
    {
        if (_all.ContainsKey(id))
            return false;

        _players[id] = entity;
        _all[id] = entity;
        return true;
    }

    /// <summary>Npc/monster map — matches historical Dictionary.Add (throws on duplicate id).</summary>
    public void RegisterNpc(int id, T entity)
    {
        _npcs.Add(id, entity);
        _all.Add(id, entity);
    }

    public void RegisterItem(int id, T entity)
    {
        _all[id] = entity;
    }

    public void Unregister(int id)
    {
        _players.Remove(id);
        _npcs.Remove(id);
        _all.Remove(id);
    }

    public void Clear()
    {
        _all.Clear();
        _players.Clear();
        _npcs.Clear();
    }

    public void ForEach(Action<T> action)
    {
        if (action == null)
            return;

        foreach (KeyValuePair<int, T> pair in _all)
        {
            if (pair.Value != null)
                action(pair.Value);
        }
    }

    public void ForEachPlayer(Action<T> action)
    {
        if (action == null)
            return;

        foreach (KeyValuePair<int, T> pair in _players)
        {
            if (pair.Value != null)
                action(pair.Value);
        }
    }

    public void ForEachNpc(Action<T> action)
    {
        if (action == null)
            return;

        foreach (KeyValuePair<int, T> pair in _npcs)
        {
            if (pair.Value != null)
                action(pair.Value);
        }
    }
}
