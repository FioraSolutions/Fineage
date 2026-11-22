using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// Batched camera→head probes for <see cref="NameplateEntryStore"/> visibility.
/// </summary>
public static class L2NameplateOcclusionBatch
{
    public const int MaxProbesPerFrame = L2NameplateScreenLayout.OverlapSphereBufferSize;

    struct Probe
    {
        public int EntryId;
        public Vector3 Anchor;
        public Transform OwnerRoot;
        public float Distance;
    }

    static readonly List<Probe> ProbeBuffer = new List<Probe>(MaxProbesPerFrame);

    public static void ApplyHeadOcclusion(
        Dictionary<int, NameplateEntry> entries,
        Camera camera,
        LayerMask occlusionMask,
        NameplateBubbleResolver resolver)
    {
        if (camera == null || entries == null || entries.Count == 0)
            return;

        ProbeBuffer.Clear();
        Vector3 camPos = camera.transform.position;

        foreach (KeyValuePair<int, NameplateEntry> kv in entries)
        {
            if (ProbeBuffer.Count >= MaxProbesPerFrame)
                break;

            NameplateEntry e = kv.Value;
            if (!e.Visible || e.Target == null)
                continue;

            if (DomainPacketServices.LocalPlayer != null && e.Entity == DomainPacketServices.LocalPlayer)
                continue;

            if (resolver != null && resolver.IsHoverOrTarget(e.Target))
                continue;

            Transform ownerRoot = e.Entity != null ? e.Entity.transform : e.Target;
            Vector3 anchor = e.Entity != null
                ? L2OverheadAnchor.GetWorldPos(e.Entity, e.Target)
                : e.Target.position;

            if (!L2NameplateOcclusionDecision.IsOnScreen(camera, anchor))
            {
                e.Visible = false;
                entries[kv.Key] = e;
                continue;
            }

            float dist = Vector3.Distance(camPos, anchor);
            ProbeBuffer.Add(new Probe
            {
                EntryId = kv.Key,
                Anchor = anchor,
                OwnerRoot = ownerRoot,
                Distance = dist,
            });
        }

        int count = ProbeBuffer.Count;
        if (count == 0)
            return;

        var commands = new NativeArray<RaycastCommand>(count, Allocator.TempJob);
        var hits = new NativeArray<RaycastHit>(count, Allocator.TempJob);

        for (int i = 0; i < count; i++)
        {
            Probe p = ProbeBuffer[i];
            Vector3 dir = p.Anchor - camPos;
            float len = dir.magnitude;
            if (len < 1e-4f)
            {
                commands[i] = default;
                continue;
            }

            dir /= len;
            commands[i] = new RaycastCommand(camPos, dir, len, occlusionMask, maxHits: 1);
        }

        var handle = RaycastCommand.ScheduleBatch(commands, hits, 1, default);
        handle.Complete();

        for (int i = 0; i < count; i++)
        {
            Probe p = ProbeBuffer[i];
            if (!entries.TryGetValue(p.EntryId, out NameplateEntry e) || !e.Visible)
                continue;

            RaycastHit hit = hits[i];
            bool headVisible = L2NameplateOcclusionDecision.IsHeadAnchorVisible(
                hit.distance,
                p.Distance,
                hit.collider,
                p.OwnerRoot);

            if (!headVisible)
            {
                e.Visible = false;
                entries[p.EntryId] = e;
            }
        }

        commands.Dispose();
        hits.Dispose();
    }
}