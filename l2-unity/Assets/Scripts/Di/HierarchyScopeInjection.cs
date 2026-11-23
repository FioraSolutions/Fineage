using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VContainer;

/// <summary>
/// Injects a shared character hierarchy from one scope.
/// A component is injected only when every <c>[Inject]</c> dependency is registered there.
/// Motion services are the lobby contract: a missing one throws.
/// A component that mixes a service this scope can resolve with one it cannot throws and names
/// the missing type — that dependency has to be split. A component whose dependencies all belong
/// to another scope (game <c>World</c> on a lobby pawn) is left for that scope.
/// </summary>
public static class HierarchyScopeInjection
{
    static IObjectResolver _lobby;

    public static IObjectResolver Lobby => _lobby;

    public static void BindLobby(IObjectResolver scope)
    {
        if (scope == null)
        {
            throw new InvalidOperationException(
                "[LobbyMotion] lobby container missing — LoginLifetimeScope built a null container.");
        }

        _lobby = scope;
    }

    public static void UnbindLobby(IObjectResolver scope)
    {
        if (ReferenceEquals(_lobby, scope))
            _lobby = null;
    }

    public static void Inject(IObjectResolver scope, GameObject root)
    {
        Classify(scope, root, null, null, true);
    }

    public static IReadOnlyList<Type> RequiredTypes(IObjectResolver scope, GameObject root)
    {
        var required = new List<Type>();
        Classify(scope, root, required, null, false);
        return required;
    }

    public static IReadOnlyList<Type> OtherScopeTypes(IObjectResolver scope, GameObject root)
    {
        var other = new List<Type>();
        Classify(scope, root, null, other, false);
        return other;
    }

    static void Classify(
        IObjectResolver scope,
        GameObject root,
        List<Type> required,
        List<Type> other,
        bool inject)
    {
        if (scope == null)
        {
            throw new InvalidOperationException(
                "[LobbyMotion] lobby container missing — LoginLifetimeScope did not build, so the pawn was not injected.");
        }

        if (root == null)
            return;

        MonoBehaviour[] behaviours = root.GetComponentsInChildren<MonoBehaviour>(true);
        for (int i = 0; i < behaviours.Length; i++)
        {
            MonoBehaviour behaviour = behaviours[i];
            if (behaviour == null)
                continue;
            ClassifyComponent(scope, behaviour, required, other, inject);
        }
    }

    static void ClassifyComponent(
        IObjectResolver scope,
        MonoBehaviour behaviour,
        List<Type> required,
        List<Type> other,
        bool inject)
    {
        List<Type> deps = CollectInjectTypes(behaviour.GetType());
        if (deps.Count == 0)
            return;

        var missing = new List<Type>();
        bool anyResolved = false;
        for (int i = 0; i < deps.Count; i++)
        {
            Type dep = deps[i];
            if (CanResolve(scope, dep))
                anyResolved = true;
            else
                missing.Add(dep);
        }

        if (missing.Count == 0)
        {
            AddDistinct(required, deps);
            if (!inject)
                return;

            try
            {
                scope.Inject(behaviour);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "[LobbyMotion] " + behaviour.GetType().Name +
                    " could not be injected (" + ex.Message + ").",
                    ex);
            }

            return;
        }

        bool missingMotion = false;
        for (int i = 0; i < missing.Count; i++)
        {
            if (MotionPipelineRegistration.IsMotionService(missing[i]))
                missingMotion = true;
        }

        if (missingMotion || anyResolved)
        {
            throw new InvalidOperationException(
                "[LobbyMotion] " + behaviour.GetType().Name + " missing " + Format(missing) +
                " — the lobby scope does not register that type, so the pawn was not injected.");
        }

        AddDistinct(other, missing);
    }

    static bool CanResolve(IObjectResolver scope, Type type)
    {
        try
        {
            object resolved;
            return scope.TryResolve(type, out resolved);
        }
        catch (VContainerException)
        {
            return false;
        }
    }

    static List<Type> CollectInjectTypes(Type type)
    {
        var deps = new List<Type>();
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        for (Type current = type; current != null && current != typeof(MonoBehaviour) && current != typeof(object); current = current.BaseType)
        {
            FieldInfo[] fields = current.GetFields(flags);
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].IsDefined(typeof(InjectAttribute), false))
                    AddDep(deps, fields[i].FieldType);
            }

            PropertyInfo[] properties = current.GetProperties(flags);
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].IsDefined(typeof(InjectAttribute), false))
                    AddDep(deps, properties[i].PropertyType);
            }

            MethodInfo[] methods = current.GetMethods(flags);
            for (int i = 0; i < methods.Length; i++)
            {
                if (!methods[i].IsDefined(typeof(InjectAttribute), false))
                    continue;
                ParameterInfo[] parameters = methods[i].GetParameters();
                for (int p = 0; p < parameters.Length; p++)
                    AddDep(deps, parameters[p].ParameterType);
            }
        }

        return deps;
    }

    static void AddDep(List<Type> deps, Type type)
    {
        if (type == null || type.ContainsGenericParameters)
            return;
        if (!deps.Contains(type))
            deps.Add(type);
    }

    static void AddDistinct(List<Type> into, List<Type> types)
    {
        if (into == null || types == null)
            return;
        for (int i = 0; i < types.Count; i++)
        {
            if (!into.Contains(types[i]))
                into.Add(types[i]);
        }
    }

    static string Format(List<Type> types)
    {
        var names = new string[types.Count];
        for (int i = 0; i < types.Count; i++)
            names[i] = types[i].Name;
        return string.Join(", ", names);
    }
}
