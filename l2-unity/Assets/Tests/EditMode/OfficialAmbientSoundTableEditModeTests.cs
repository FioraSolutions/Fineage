using System.Text;
using NUnit.Framework;
using UnityEngine;

public class OfficialAmbientSoundTableEditModeTests
{
    [SetUp]
    public void SetUp()
    {
        OfficialAmbientSoundTable.ClearForTests();
        OfficialWaveTable.ClearForTests();
    }

    [TearDown]
    public void TearDown()
    {
        OfficialAmbientSoundTable.ClearForTests();
        OfficialWaveTable.ClearForTests();
    }

    [Test]
    public void Loads_official_rows_and_converts_units()
    {
        OfficialAmbientSoundTable.LoadFrom(
            "map\tname\tsound\tx\ty\tz\tradius\tvolume\tpitch\ttype\trandom\tstart\n"
            + "17_25\tAmbientSoundObject1\tAmbSound.Insect_Night.in_cricket_03\t-91331.8\t252042.8\t-3374.0\t80\t250\t55\tAST1_Night\t40\t12.5\n"
            + "17_25\tAmbientSoundObject2\tAmbSound.Waves.small_03\t-90000\t252000\t-3300\t150\t255\t0\t2\t0\t0\n",
            "test://official-ambient-objects.tsv");

        Assert.AreEqual(2, OfficialAmbientSoundTable.Count);
        Assert.IsTrue(OfficialAmbientSoundTable.HasActors);
        OfficialAmbientSoundTable.Entry cricket = OfficialAmbientSoundTable.ForMap("17_25")[0];
        Assert.AreEqual("AmbSound.Insect_Night.in_cricket_03", cricket.Sound);
        Assert.AreEqual(AmbientSoundType.AST1_Night, cricket.Type);
        Assert.AreEqual(80f, cricket.RadiusUu);
        Assert.AreEqual(80f * VectorUtils.L2UuToUnity, cricket.RadiusUnity, 0.0001f);
        Vector3 unity = cricket.UnityPosition;
        Vector3 expected = VectorUtils.ConvertPosToUnity(new Vector3(-91331.8f, 252042.8f, -3374.0f));
        Assert.AreEqual(expected.x, unity.x, 0.01f);
        Assert.AreEqual(expected.y, unity.y, 0.01f);
        Assert.AreEqual(expected.z, unity.z, 0.01f);
        Assert.AreEqual(AmbientSoundType.AST1_Night, OfficialAmbientSoundTable.ForMap("17_25")[1].Type);
        Assert.AreEqual(1.55f, OfficialAmbientSoundUnits.PitchToMultiplier(55f), 0.0001f);
        Assert.AreEqual(250f / 255f, OfficialAmbientSoundUnits.VolumeToScale(250f), 0.0001f);
    }

    [Test]
    public void Resolves_official_ambient_dat_tokens()
    {
        OfficialWaveTable.LoadFrom(
            "package\tgroup\tname\tresources_path\n"
            + "ambsound\tInsect_Night\tin_cricket_03\tData/Sounds/AmbSound/Insect_Night/in_cricket_03\n"
            + "AmbSound2\tWeather\train_01\tData/Sounds/AmbSound2/Weather/rain_01\n"
            + "ambsound\tWeather\train_01\tData/Sounds/AmbSound/Weather/rain_01\n",
            "test://official-waves.tsv");

        Assert.IsTrue(OfficialWaveTable.TryResolveDatToken(
            "AmbSound.Insect_Night.in_cricket_03", out string cricket, out _));
        Assert.AreEqual("Data/Sounds/AmbSound/Insect_Night/in_cricket_03", cricket);
        Assert.IsTrue(OfficialWaveTable.TryResolveDatToken(
            "AmbSound2.Weather.rain_01", out string rain2, out _));
        Assert.AreEqual("Data/Sounds/AmbSound2/Weather/rain_01", rain2);
        Assert.IsFalse(OfficialWaveTable.TryResolve("rain_01", out _, out string error));
        Assert.IsTrue(error.Contains("ambiguous") || error.Contains("rain_01"), error);
    }

    [Test]
    public void Missing_table_logs_concrete_path()
    {
        string logged = null;
        OfficialAmbientSoundTable.ErrorLogged = msg => logged = msg;
        OfficialAmbientSoundTable.LoadFrom((byte[])null, "test://missing.tsv.gz");
        Assert.IsNotNull(logged);
        Assert.IsTrue(logged.Contains("path=\"test://missing.tsv.gz\""), logged);
        Assert.IsFalse(OfficialAmbientSoundTable.HasActors);
    }
}
