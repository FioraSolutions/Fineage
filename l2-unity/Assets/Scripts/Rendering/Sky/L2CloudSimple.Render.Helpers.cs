using System;
using UnityEngine;
using UnityEngine.Rendering;

public partial class L2CloudSimple : MonoBehaviour
{
    static void SetActive(ref CloudChunk chunk, bool draw)
    {
        if (chunk.mr != null)
            chunk.mr.enabled = draw;
        if (chunk.tf != null)
            chunk.tf.gameObject.SetActive(draw);
    }

    void EnsureChunks()
    {
        Load(ref _domeA0, TexDomeA0);
        Load(ref _domeA1, TexDomeA1);
        Load(ref _domeB0, TexDomeB0);
        Load(ref _domeB1, TexDomeB1);
        if (_capMesh == null)
            _capMesh = LoadPackageMesh(MeshCapPath);
        if (_beltMesh == null)
            _beltMesh = LoadPackageMesh(MeshBeltPath);
        Shader shader = Shader.Find(ShaderName);
        if (shader == null || _domeA0 == null || _domeA1 == null || _domeB0 == null || _domeB1 == null ||
            _capMesh == null || _beltMesh == null)
            return;

        Wrap(_domeA0, TextureWrapMode.Repeat, TextureWrapMode.Repeat);
        Wrap(_domeA1, TextureWrapMode.Repeat, TextureWrapMode.Repeat);
        Wrap(_domeB0, TextureWrapMode.Repeat, TextureWrapMode.Clamp);
        Wrap(_domeB1, TextureWrapMode.Repeat, TextureWrapMode.Clamp);

        EnsureChunk(ref _domeA, "L2CloudSimple_Cap", _capMesh, 2);
        EnsureChunk(ref _domeB, "L2CloudSimple_Ring", _beltMesh, 2);

        if (_domeA.mats == null)
        {
            _domeA.mats = new[]
            {
                MakeMat(shader, _domeA0, DaySheetTint, 2970, true, SkyCapRadius),
                MakeMat(shader, _domeA1, DaySheetTint, 2971, true, SkyCapRadius)
            };
            _domeA.mr.sharedMaterials = _domeA.mats;
        }

        if (_domeB.mats == null)
        {
            _domeB.mats = new[]
            {
                MakeMat(shader, _domeB0, DaySheetTint, 2973, true, SkyBeltRadius),
                MakeMat(shader, _domeB1, DaySilhouetteTint, 2974, false, SkyBeltRadius)
            };
            _domeB.mr.sharedMaterials = _domeB.mats;
        }
    }

    static void Load(ref Texture2D slot, string path)
    {
        if (slot == null)
            slot = Resources.Load<Texture2D>(path);
    }
}
