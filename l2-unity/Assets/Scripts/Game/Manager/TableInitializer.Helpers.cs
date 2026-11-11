using VContainer;

public sealed partial class TableInitializer
{
    private void WireTablePacketServices()
    {
        TablePacketServices.Bind(
            items: _items,
            itemNames: _itemNames,
            itemStats: _itemStats,
            armorGrps: _armorGrps,
            etcItemGrps: _etcItemGrps,
            weaponGrps: _weaponGrps,
            npcDecoEffects: _npcDecoEffects,
            npcGrps: _npcGrps,
            npcNames: _npcNames,
            questNames: _questNames,
            recipes: _recipes,
            actionNames: _actionNames,
            animLengths: _animLengths,
            sysStrings: _sysStrings,
            skillNames: _skillNames,
            skillGrps: _skillGrps,
            mobSkillAnimGrps: _mobSkillAnimGrps,
            animTweens: _animTweens,
            enterEvents: _enterEvents,
            models: _models,
            logonGrps: _logonGrps,
            systemMessages: _systemMessages);
    }
}
