using UnityEngine;

/// <summary>Maps <see cref="RuntimePlatform"/> to <see cref="GraphicsPlatformGroup"/> from a single table (no scattered platform branches).</summary>
public static class GraphicsQualityPlatformTable
{
    readonly struct Binding
    {
        public readonly RuntimePlatform Platform;
        public readonly GraphicsPlatformGroup Group;

        public Binding(RuntimePlatform platform, GraphicsPlatformGroup group)
        {
            Platform = platform;
            Group = group;
        }
    }

    static readonly Binding[] Table =
    {
        new(RuntimePlatform.WindowsPlayer, GraphicsPlatformGroup.Desktop),
        new(RuntimePlatform.OSXPlayer, GraphicsPlatformGroup.Desktop),
        new(RuntimePlatform.LinuxPlayer, GraphicsPlatformGroup.Desktop),
        new(RuntimePlatform.WindowsEditor, GraphicsPlatformGroup.Desktop),
        new(RuntimePlatform.OSXEditor, GraphicsPlatformGroup.Desktop),
        new(RuntimePlatform.LinuxEditor, GraphicsPlatformGroup.Desktop),
        new(RuntimePlatform.Android, GraphicsPlatformGroup.Mobile),
        new(RuntimePlatform.IPhonePlayer, GraphicsPlatformGroup.Mobile),
    };

    public static bool TryResolve(RuntimePlatform platform, out GraphicsPlatformGroup group)
    {
        for (int i = 0; i < Table.Length; i++)
        {
            if (Table[i].Platform == platform)
            {
                group = Table[i].Group;
                return true;
            }
        }

        group = GraphicsPlatformGroup.Desktop;
        return false;
    }

    public static GraphicsPlatformGroup ResolveOrDefault(RuntimePlatform platform)
    {
        return TryResolve(platform, out GraphicsPlatformGroup group) ? group : GraphicsPlatformGroup.Desktop;
    }
}