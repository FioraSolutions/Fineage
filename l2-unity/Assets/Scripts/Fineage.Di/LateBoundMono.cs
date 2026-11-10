using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="ILateBound{T}"/> for a MonoBehaviour that appears after the scope is built
/// (#142 slice 4). Lookup order matches the existing <c>RegisterLiveMono</c> contract:
/// the container's own registration when one exists at build time, then a live scene
/// search, then the static <c>Instance</c> the type exposes. Every step may legitimately
/// return null; callers must treat that as "not created yet", not as an error.
/// </summary>
public sealed class LateBoundMono<T> : ILateBound<T> where T : MonoBehaviour
{
    private readonly Func<T> _instance;
    private T _value;

    public LateBoundMono(Func<T> instance)
    {
        _instance = instance;
    }

    public T Value
    {
        get
        {
            if (_value != null)
                return _value;

            if (_instance != null)
                _value = _instance();

            if (_value == null)
                _value = UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);

            return _value;
        }
    }
}
