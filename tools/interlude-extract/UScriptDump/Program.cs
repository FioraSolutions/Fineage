using UELib;
using UELib.Core;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: UScriptDump <package.u> <outDir>");
    return 2;
}

string packagePath = args[0];
string outDir = args[1];
Directory.CreateDirectory(outDir);

var package = UnrealLoader.LoadPackage(packagePath, FileAccess.Read);
package.InitializePackage();

int ok = 0, fail = 0, skipped = 0;
foreach (var obj in package.Objects)
{
    if (obj is not UClass cls)
        continue;
    if (string.IsNullOrEmpty(cls.Name) || string.Equals(cls.Name, "None", StringComparison.OrdinalIgnoreCase))
    {
        skipped++;
        continue;
    }
    try
    {
        cls.BeginDeserializing();
    }
    catch
    {
        // some classes have no script; still try decompile
    }
    string text;
    try
    {
        text = cls.Decompile();
    }
    catch (Exception ex)
    {
        fail++;
        Console.Error.WriteLine("FAIL " + cls.Name + " " + ex.GetType().Name + ": " + ex.Message);
        continue;
    }
    if (string.IsNullOrWhiteSpace(text))
    {
        skipped++;
        continue;
    }
    string file = Path.Combine(outDir, cls.Name + ".uc");
    File.WriteAllText(file, text);
    ok++;
}
Console.WriteLine($"wrote={ok} fail={fail} skipped={skipped} package={packagePath}");
return fail > 0 && ok == 0 ? 1 : 0;
