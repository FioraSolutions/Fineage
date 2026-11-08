using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UELib;
using UELib.Core;
using UELib.Types;

static class Program
{
    static int Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine(
                "Usage: UeDecompile <package.u> <outDir> <className> [className...]");
            return 2;
        }

        UnrealConfig.VariableTypes = new Dictionary<string, Tuple<string, PropertyType>>(
            StringComparer.OrdinalIgnoreCase)
        {
            { "Emitters", Tuple.Create("ObjectProperty", PropertyType.ObjectProperty) },
            { "ColorScale", Tuple.Create("StructProperty", PropertyType.StructProperty) },
            { "SizeScale", Tuple.Create("StructProperty", PropertyType.StructProperty) },
            { "RevolutionScale", Tuple.Create("StructProperty", PropertyType.StructProperty) },
            { "CustomMaterials", Tuple.Create("ObjectProperty", PropertyType.ObjectProperty) },
        };

        string packagePath = args[0];
        string outDir = args[1];
        Directory.CreateDirectory(outDir);

        var package = UnrealLoader.LoadPackage(packagePath, FileAccess.Read);
        package.InitializePackage();

        // Map Outer name -> child objects (emitters)
        var childrenByOuter = new Dictionary<string, List<UObject>>(StringComparer.OrdinalIgnoreCase);
        foreach (var obj in package.Objects)
        {
            if (obj == null || obj.Outer == null)
                continue;
            string outerName = obj.Outer.Name;
            if (!childrenByOuter.TryGetValue(outerName, out var list))
            {
                list = new List<UObject>();
                childrenByOuter[outerName] = list;
            }
            list.Add(obj);
        }

        int ok = 0;
        for (int i = 2; i < args.Length; i++)
        {
            string className = args[i];
            var cls = package.Objects
                .OfType<UClass>()
                .FirstOrDefault(o =>
                    string.Equals(o.Name, className, StringComparison.OrdinalIgnoreCase));
            if (cls == null)
            {
                Console.Error.WriteLine("MISSING " + className);
                continue;
            }

            var sb = new StringBuilder();
            string parent = cls.Super != null ? cls.Super.Name : "Emitter";
            sb.AppendLine("class " + className + " extends " + parent + ";");
            sb.AppendLine();
            sb.AppendLine("defaultproperties");
            sb.AppendLine("{");

            childrenByOuter.TryGetValue(className, out var kids);
            kids ??= new List<UObject>();
            // Prefer particle emitter-like children
            var emitters = kids
                .Where(k =>
                {
                    string cn = k.Class != null ? k.Class.Name : k.GetType().Name;
                    return cn.IndexOf("Emitter", StringComparison.OrdinalIgnoreCase) >= 0;
                })
                .ToList();

            Console.WriteLine(
                className + " children=" + kids.Count +
                " emitters=" + emitters.Count +
                " types=[" + string.Join(",", kids.Select(k =>
                    (k.Class != null ? k.Class.Name : "?") + ":" + k.Name).Take(20)) + "]");

            int emitterIndex = 0;
            foreach (var em in emitters)
            {
                string emClass = em.Class != null ? em.Class.Name : "SpriteEmitter";
                string emName = em.Name;
                string body = SafeDecompile(em);
                // Strip class wrapper if present; keep defaultproperties body
                string inner = ExtractDefaultPropertiesBody(body);
                sb.AppendLine("     Begin Object Class=" + emClass + " Name=" + emName);
                if (!string.IsNullOrWhiteSpace(inner))
                {
                    foreach (string line in inner.Split('\n'))
                    {
                        string t = line.TrimEnd('\r');
                        if (string.IsNullOrWhiteSpace(t))
                            continue;
                        sb.AppendLine("         " + t.TrimStart());
                    }
                }
                sb.AppendLine("     End Object");
                sb.AppendLine("     Emitters(" + emitterIndex + ")=" + emClass + "'" + emName + "'");
                emitterIndex++;
            }

            // Class-level properties from partial decompile (Speed, DrawScale, etc.)
            string classUc = SafeDecompile(cls);
            foreach (string line in ExtractDefaultPropertiesBody(classUc).Split('\n'))
            {
                string t = line.Trim().TrimEnd('\r');
                if (string.IsNullOrWhiteSpace(t))
                    continue;
                if (t.StartsWith("Emitters=", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (t.StartsWith("Begin Object", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (t.StartsWith("End Object", StringComparison.OrdinalIgnoreCase))
                    continue;
                sb.AppendLine("     " + t);
            }

            sb.AppendLine("}");

            string path = Path.Combine(outDir, className + ".uc");
            File.WriteAllText(path, sb.ToString());
            Console.WriteLine("OK " + className + " -> " + path + " (" + sb.Length + " chars)");
            ok++;
        }

        return ok > 0 ? 0 : 1;
    }

    static string SafeDecompile(UObject obj)
    {
        try { return obj.Decompile() ?? string.Empty; }
        catch (Exception ex) { return "/* decompile error: " + ex.Message + " */"; }
    }

    static string ExtractDefaultPropertiesBody(string uc)
    {
        if (string.IsNullOrEmpty(uc))
            return string.Empty;
        int idx = uc.IndexOf("defaultproperties", StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
            return uc;
        int brace = uc.IndexOf('{', idx);
        if (brace < 0)
            return string.Empty;
        int depth = 0;
        for (int i = brace; i < uc.Length; i++)
        {
            if (uc[i] == '{') depth++;
            else if (uc[i] == '}')
            {
                depth--;
                if (depth == 0)
                    return uc.Substring(brace + 1, i - brace - 1);
            }
        }
        return uc.Substring(brace + 1);
    }
}
