using NUnit.Framework;
using UnityEngine;

public sealed class L2ActorPointLightsRegistryEditModeTests
{
    [Test]
    public void Local_light_registry_round_trip()
    {
        GameObject go = new GameObject("test-light");
        L2ActorLocalLight light = go.AddComponent<L2ActorLocalLight>();

        L2ActorPointLights.RegisterLocalLight(light);
        L2ActorPointLights.UnregisterLocalLight(light);

        Object.DestroyImmediate(go);
    }
}