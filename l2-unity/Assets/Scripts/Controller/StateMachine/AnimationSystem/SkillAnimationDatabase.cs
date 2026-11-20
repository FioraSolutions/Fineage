using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class SkillAnimationDatabase
{
    private static readonly Dictionary<string, SkillClips> _raceCacheMMagic = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheFMagic = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheMFighter = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheFFighter = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheMElf = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheFElf = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheMDarkElf = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheFDarkElf = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheMDwarf = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheFDwarf = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheMOrc = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheFOrc = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheMShaman = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheFShaman = new Dictionary<string, SkillClips>();
    private const string  FOLDER_MElf = "Data/Animations/Elf/MElf/Clips/";
    private const string  FOLDER_FElf = "Data/Animations/Elf/FElf/Clips/";
    private const string  FOLDER_MDarkElf = "Data/Animations/DarkElf/MDarkElf/Clips/";
    private const string  FOLDER_FDarkElf = "Data/Animations/DarkElf/FDarkElf/Clips/";
    private const string  FOLDER_MDwarf = "Data/Animations/Dwarf/MDwarf/Clips/";
    private const string  FOLDER_FDwarf = "Data/Animations/Dwarf/FDwarf/Clips/";
    private const string  FOLDER_MOrc = "Data/Animations/Orc/MOrc/Clips/";
    private const string  FOLDER_FOrc = "Data/Animations/Orc/FOrc/Clips/";
    private const string  FOLDER_MShaman = "Data/Animations/Orc/MShaman/Clips/";
    private const string  FOLDER_FShaman = "Data/Animations/Orc/FShaman/Clips/";
    private const string  FOLDER_MMagic = "Data/Animations/Magic/MMagic/Clips/";
    private const string  FOLDER_FMagic = "Data/Animations/Magic/FMagic/Clips/";
    private const string  FOLDER_MFighter = "Data/Animations/Fighter/MFighter/Clips/";
    private const string  FOLDER_FFighter = "Data/Animations/Fighter/FFighter/Clips/";
    private const string MElf = "MElf";
    private const string FElf = "FElf";
    private const string MDarkElf = "MDarkElf";
    private const string FDarkElf = "FDarkElf";
    private const string MDwarf = "MDwarf";
    private const string FDwarf = "FDwarf";
    private const string MOrc = "MOrc";
    private const string FOrc = "FOrc";
    private const string MShaman = "MShaman";
    private const string FShaman = "FShaman";
    private const string MMagic = "MMagic";
    private const string FMagic = "FMagic";
    private const string MFighter = "MFighter";
    private const string FFighter = "FFighter";
    public class SkillClips
    {
        private AnimationClip _clip;
        private string _name;
        private string _path;
        
        public SkillClips(string folder , string name)
        {
            string path = folder + name;

            _name = name;
            _path = path;
        }

        public void Load()
        {
            if(_clip == null)
            {
                _clip = Resources.Load<AnimationClip>(_path);
                if (_clip != null)
                {
                    // Debug.Log($"SkillAnimationDatabase> Загрузился ролик анимации  в кэш имя {_name} его путь {_path}");
                }
                else
                {
                    // Debug.LogWarning($"SkillAnimationDatabase> Не смогли загрузить в кэщ {_name} его путь {_path}");
                }

            }
        }

        public AnimationClip GetClip()
        {
            return _clip;
        }

        public string GetName()
        {
            return _name;
        }
    }
}
