using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed partial class GameLifetimeScope
{
    /// <summary>ACS PacketServices bridges for prefab-deferred MonoBehaviours.</summary>
    private static T ResolvePacketServicesFallback<T>() where T : MonoBehaviour
    {
        if (typeof(T) == typeof(PlayerInventory))
            return InventoryPacketServices.Inventory as T;
        if (typeof(T) == typeof(TargetManager))
            return TargetPacketServices.Targets as T;
        if (typeof(T) == typeof(ProjectileManager))
            return ProjectilePacketServices.Projectiles as T;
        if (typeof(T) == typeof(TradeWindow))
            return TradePacketServices.Trade as T;
        if (typeof(T) == typeof(TradeRequestWindow))
            return TradePacketServices.TradeRequest as T;
        if (typeof(T) == typeof(PartyInvitationWindow))
            return PartyPacketServices.PartyInvite as T;
        if (typeof(T) == typeof(SkillListWindow))
            return SkillLearnPacketServices.SkillList as T;
        if (typeof(T) == typeof(BufferPanel))
            return AuxiliaryGameUIPacketServices.Buffer as T;
        if (typeof(T) == typeof(MoveAllCharacters))
            return MoveAllCharactersPacketServices.Moves as T;
        if (typeof(T) == typeof(EffectManager))
            return GlobalEffectsPacketServices.Effects as T;
        if (typeof(T) == typeof(EventBus))
            return ControllerPacketServices.Bus as T;
        if (typeof(T) == typeof(SkillbarWindow))
            return SkillbarPacketServices.Skillbar as T;
        if (typeof(T) == typeof(ChatWindow))
            return MessagePacketServices.Chat as T;
        if (typeof(T) == typeof(HtmlWindow))
            return GameWindowPacketServices.Html as T;
        if (typeof(T) == typeof(SystemMessageWindow))
            return SystemMessagePacketServices.Window as T;
        if (typeof(T) == typeof(CameraController))
            return CameraPacketServices.Controller as T;
        if (typeof(T) == typeof(CombatFacingService))
            return CombatPacketServices.Facing as T;
        if (typeof(T) == typeof(CharacterInfoWindow))
            return HudCorePacketServices.CharacterInfo as T;
        if (typeof(T) == typeof(CharacterBuilder))
            return CharacterPacketServices.Builder as T;

        return null;
    }
}
