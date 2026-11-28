using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Fineage.Net.Contract;

/// <summary>
/// <see cref="CombatTrace"/> helpers for the packet flow: opcode names and a one-line dump of a decoded body's
/// scalar properties (<c>EntityValue</c> is printed as <c>id</c>, the zone object id).
/// </summary>
public static class NetPacketTrace
{
    static Dictionary<ushort, string> _names;
    static readonly Dictionary<Type, PropertyInfo[]> Props = new Dictionary<Type, PropertyInfo[]>();
    const int MaxProps = 14;

    public static string OpName(ushort opcode)
    {
        if (_names == null)
        {
            var names = new Dictionary<ushort, string>();
            AddConsts(names, typeof(NetOpcode));
            AddConsts(names, typeof(NetOpcodeMobius));
            _names = names;
        }
        return _names.TryGetValue(opcode, out string n) ? n : "0x" + opcode.ToString("X4");
    }

    static void AddConsts(Dictionary<ushort, string> names, Type t)
    {
        foreach (FieldInfo f in t.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (f.IsLiteral && f.FieldType == typeof(ushort))
            {
                ushort v = (ushort)f.GetRawConstantValue();
                if (!names.ContainsKey(v))
                    names[v] = f.Name;
            }
        }
    }

    public static string Describe(object body)
    {
        if (body == null)
            return string.Empty;
        PropertyInfo[] props;
        Type type = body.GetType();
        lock (Props)
        {
            if (!Props.TryGetValue(type, out props))
            {
                var list = new List<PropertyInfo>();
                foreach (PropertyInfo p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    Type pt = p.PropertyType;
                    if (p.GetIndexParameters().Length == 0 && (pt.IsPrimitive || pt.IsEnum || pt == typeof(string)))
                        list.Add(p);
                    if (list.Count >= MaxProps)
                        break;
                }
                props = list.ToArray();
                Props[type] = props;
            }
        }

        var sb = new StringBuilder(96);
        for (int i = 0; i < props.Length; i++)
        {
            object v;
            try { v = props[i].GetValue(body); }
            catch (Exception) { continue; }
            if (v == null)
                continue;
            string name = props[i].Name;
            if (name == "EntityValue")
            {
                name = "id";
                v = unchecked((int)(ulong)v);
            }
            else if (name == "EntityGeneration")
                continue;
            if (sb.Length > 0)
                sb.Append(' ');
            string text = v is string s ? (s.Length > 32 ? s.Substring(0, 32) : s).Replace(' ', '_') : v.ToString();
            sb.Append(name).Append('=').Append(text);
        }
        return sb.ToString();
    }
}
