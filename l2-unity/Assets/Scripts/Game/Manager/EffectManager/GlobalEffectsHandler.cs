using UnityEngine;
using VContainer;

public class GlobalEffectsHandler : MonoBehaviour
{
    [Inject] EventBus _bus;
    [Inject] EffectManager _effects;

    private void Awake()
    {
        WireGlobalEffectsPacketServices();
    }

    private void WireGlobalEffectsPacketServices()
    {
        GlobalEffectsPacketServices.Bind(bus: _bus, effects: _effects);
    }

    private void Start()
    {
        WireGlobalEffectsPacketServices();
        EventBus bus = GlobalEffectsPacketServices.Bus;
        if (bus != null)
            bus.OnLevelUp += HandleLevelUp;
    }

    private void OnDestroy()
    {
        EventBus bus = GlobalEffectsPacketServices.Bus;
        if (bus != null)
            bus.OnLevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp(Entity entity, int newLevel)
    {
        EffectManager effects = GlobalEffectsPacketServices.Effects;
        if (effects != null)
            effects.PlayEffect((int)SpecialSkillType.LevelUp, entity.transform);

        // Interlude skillsound levelup.ogg — same 2D hub as skills / MonSound.
        InterludeSfx.Play("Data/Sounds/SkillSound/levelup", 0.85f);
    }
}
