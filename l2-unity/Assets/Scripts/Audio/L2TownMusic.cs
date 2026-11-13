using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Town themes from the original client (<c>music/ct_*.ogg</c>).
/// Dark Elf keeps the cave rule: <c>ct_darkelf</c> while the camera is under the 20_18 mouth.
/// Elven, Orc and Dwarven villages use the retail peace-zone outlines.
/// </summary>
public sealed class L2TownMusic : MonoBehaviour
{
    const string DarkElf = "event:/Music/l2_lobby/ct_darkelf";
    const string Elf = "event:/Music/l2_lobby/ct_elf";
    const string Orc = "event:/Music/l2_lobby/ct_orc";
    const string Dwarf = "event:/Music/l2_lobby/ct_dwarf";
    const float CaveLipUnityY = 45f;
    const float Uu = 52.5f;

    static readonly int[] ElfTown =
    {
        47758, 51443, 46735, 52457, 45358, 52947, 44182, 52419, 43398, 51830,
        43006, 51401, 42508, 50515, 42343, 49479, 42547, 48421, 43138, 47363,
        44342, 46639, 45621, 46486, 45974, 46554, 47114, 46996, 47777, 47710,
        48202, 48441, 48400, 49391, 48365, 50345
    };

    static readonly int[] OrcTownA =
    {
        -42044, -113585, -42533, -111862, -43811, -110599, -45614, -110071,
        -47375, -110007, -47397, -117215, -46661, -117220, -45633, -117094,
        -43791, -116569, -42530, -115322
    };

    static readonly int[] OrcTownB =
    {
        -50211, -116941, -47413, -117301, -47358, -109913, -49161, -109305,
        -51245, -109819, -53401, -112501, -53788, -113966, -52931, -115293
    };

    static readonly int[][] DwarfTown =
    {
        new[] { 117520, -181718, 117423, -180017, 115371, -176774, 114885, -176815, 114070, -177180, 113883, -179469, 114580, -181875 },
        new[] { 117630, -182925, 115159, -182811, 115477, -183673, 115600, -183891, 115740, -184086, 115948, -184206, 116257, -184251, 116710, -184253, 117023, -184164, 117204, -184034, 117335, -183812, 117503, -183386 },
        new[] { 114641, -183866, 117517, -183140, 117778, -182649, 117775, -182246, 117733, -182086, 117432, -181576, 114638, -181551 },
        new[] { 116406, -176891, 115964, -176809, 115535, -176773, 115142, -176788, 115867, -178003, 117355, -179595, 117300, -178525, 117172, -177847, 116798, -177111, 116643, -176968 },
        new[] { 116649, -179760, 117429, -180911, 117493, -180052, 117467, -179575, 116977, -178211, 116142, -178429 }
    };

    EventInstance _inst;
    string _playing;

    void OnDisable() => Stop();

    void LateUpdate()
    {
        string theme = Theme();
        if (theme == null)
        {
            if (_playing != null)
                Stop();
            return;
        }

        if (_playing != theme)
        {
            Stop();
            Play(theme);
        }
        else
            KeepAlive();
    }

    static string Theme()
    {
        if (!TryListener(out Vector3 unity, out int x, out int y))
            return null;

        if (InDarkElfCave())
            return DarkElf;
        if (Inside(x, y, ElfTown))
            return Elf;
        if (Inside(x, y, OrcTownA) || Inside(x, y, OrcTownB))
            return Orc;
        for (int i = 0; i < DwarfTown.Length; i++)
        {
            if (Inside(x, y, DwarfTown[i]))
                return Dwarf;
        }

        return null;
    }

    static bool InDarkElfCave()
    {
        Scene tile = SceneManager.GetSceneByName("20_18");
        if (!tile.IsValid() || !tile.isLoaded)
            return false;

        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player != null)
            return player.transform.position.y < CaveLipUnityY;

        Camera cam = Camera.main;
        return cam != null && cam.transform.position.y < CaveLipUnityY;
    }

    static bool TryListener(out Vector3 unity, out int x, out int y)
    {
        unity = default;
        x = 0;
        y = 0;
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player != null)
            unity = player.transform.position;
        else if (Camera.main != null)
            unity = Camera.main.transform.position;
        else
            return false;

        // Unity X is L2 Y, Unity Z is L2 X.
        x = Mathf.RoundToInt(unity.z * Uu);
        y = Mathf.RoundToInt(unity.x * Uu);
        return true;
    }

    static bool Inside(int x, int y, int[] ring)
    {
        bool inside = false;
        int count = ring.Length / 2;
        int j = count - 1;
        for (int i = 0; i < count; i++)
        {
            int xi = ring[i * 2];
            int yi = ring[i * 2 + 1];
            int xj = ring[j * 2];
            int yj = ring[j * 2 + 1];
            if ((yi > y) != (yj > y))
            {
                float cross = (xj - xi) * (y - yi) / (float)(yj - yi) + xi;
                if (x < cross)
                    inside = !inside;
            }

            j = i;
        }

        return inside;
    }

    void Play(string path)
    {
        try
        {
            EventReference er = RuntimeManager.PathToEventReference(path);
            if (er.IsNull)
                return;

            _inst = RuntimeManager.CreateInstance(er);
            _inst.setVolume(GameSettings.Music);
            _inst.start();
            _playing = path;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[TownMusic] " + path + " failed: " + ex.Message);
        }
    }

    void KeepAlive()
    {
        if (!_inst.isValid())
        {
            _playing = null;
            return;
        }

        _inst.setVolume(GameSettings.Music);
        _inst.getPlaybackState(out PLAYBACK_STATE state);
        if (state == PLAYBACK_STATE.STOPPED)
            _inst.start();
    }

    void Stop()
    {
        _playing = null;
        if (!_inst.isValid())
            return;

        _inst.stop(STOP_MODE.ALLOWFADEOUT);
        _inst.release();
        _inst.clearHandle();
    }
}
