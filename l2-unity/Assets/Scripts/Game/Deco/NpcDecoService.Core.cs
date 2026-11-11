using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class NpcDecoService
{
    public NpcDecoService(NpcDecoCatalog catalog)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _hostSettings = ScriptableObject.CreateInstance<EffectParticle>();
        _hostSettings.isFollowCaster = true;
        _hostSettings.defaultLifeTime = 3600f;
        _hostSettings.hideTime = 0f;
    }

    public void Start(Entity entity)
    {
        if (!Enabled)
            return;
        if (entity == null || entity.Identity == null)
            return;

        int entityId = entity.Identity.Id;
        Stop(entityId);

        NpcDecoEffect deco = ResolveDeco(entity);
        if (deco == null || !deco.HasEffectName)
            return;

        if (!_catalog.TryLoadPieces(deco.DecoEffect, _loadBuffer))
        {
            if (_missingLogged.Add(deco.DecoEffect))
            {
                // Debug.LogWarning(
                    // "NpcDecoService: prefab not found for '" + deco.DecoEffect +
                    // "' under Resources/" + NpcDecoCatalog.ResourcesFolder +
                    // " (expected " + NpcDecoCatalog.ShortName(deco.DecoEffect) +
                    // " or folder of pieces)");
            }
            return;
        }

        List<BaseEffect> spawned = new List<BaseEffect>(_loadBuffer.Count);
        for (int i = 0; i < _loadBuffer.Count; i++)
        {
            BaseEffect instance = SpawnPiece(entity, _loadBuffer[i]);
            if (instance != null)
                spawned.Add(instance);
        }

        if (spawned.Count > 0)
            _byEntityId[entityId] = spawned;
    }

    public void Stop(int entityId)
    {
        if (!_byEntityId.TryGetValue(entityId, out List<BaseEffect> spawned))
            return;

        _byEntityId.Remove(entityId);
        if (spawned == null)
            return;

        for (int i = 0; i < spawned.Count; i++)
        {
            BaseEffect instance = spawned[i];
            if (instance == null)
                continue;

            ParticleEmitterV2.StopAll(instance);
            UnityEngine.Object.Destroy(instance.gameObject);
        }
    }
}
