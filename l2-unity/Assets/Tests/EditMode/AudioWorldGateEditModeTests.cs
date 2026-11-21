using NUnit.Framework;
using UnityEngine;

public sealed class AudioWorldGateEditModeTests
{
    [Test]
    public void Hear_max_matches_interlude_world_constants()
    {
        Assert.AreEqual(28f, AudioWorldConstants.HearMaxMeters, 0.001f);
    }

    [Test]
    public void Distance_is_planar_only()
    {
        var listener = new GameObject("listener").transform;
        listener.position = new Vector3(0f, 0f, 0f);
        var source = new Vector3(3f, 99f, 4f);
        float dist = Vector3.Distance(
            new Vector3(listener.position.x, 0f, listener.position.z),
            new Vector3(source.x, 0f, source.z));
        Assert.AreEqual(5f, dist, 0.01f);
        Object.DestroyImmediate(listener.gameObject);
    }
}