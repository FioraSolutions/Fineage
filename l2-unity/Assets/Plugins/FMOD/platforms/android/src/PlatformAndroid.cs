using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
namespace FMOD
{
    public partial class VERSION
    {
        public const string dll = "fmod";
    }
}

namespace FMOD.Studio
{
    public partial class STUDIO_VERSION
    {
        public const string dll = "fmodstudio";
    }
}
#endif

namespace FMODUnity
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class PlatformAndroid : Platform
    {
        static PlatformAndroid()
        {
            Settings.AddPlatformTemplate<PlatformAndroid>("b7e1c4a09d2f4e6a8c1b3d5e7f90a214");
        }

        internal override string DisplayName { get { return "Android"; } }

        internal override void DeclareRuntimePlatforms(Settings settings)
        {
            settings.DeclareRuntimePlatform(RuntimePlatform.Android, this);
        }

        public static string StaticGetBankFolder()
        {
            return "file:///android_asset";
        }

        public static string StaticGetPluginPath(string pluginName)
        {
            return "lib" + pluginName + ".so";
        }

#if UNITY_EDITOR
        internal override OutputType[] ValidOutputTypes { get { return null; } }
#endif

#if UNITY_EDITOR
        internal override IEnumerable<BuildTarget> GetBuildTargets()
        {
            yield return BuildTarget.Android;
        }

        internal override Legacy.Platform LegacyIdentifier { get { return Legacy.Platform.Android; } }

        protected override BinaryAssetFolderInfo GetBinaryAssetFolder(BuildTarget buildTarget)
        {
            return new BinaryAssetFolderInfo("android", "Plugins");
        }

        protected override IEnumerable<FileRecord> GetBinaryFiles(BuildTarget buildTarget, bool allVariants, string suffix)
        {
            yield break;
        }
#endif
    }
}
