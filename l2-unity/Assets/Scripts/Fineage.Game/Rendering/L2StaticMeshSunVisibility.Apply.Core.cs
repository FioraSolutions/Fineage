using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class L2StaticMeshSunVisibility
{
    public static MaskSet Bake(
        Mesh mesh,
        Transform meshTransform,
        PhysicsScene physicsScene,
        ICollection<Collider> ownerColliders,
        int collisionMask = Physics.DefaultRaycastLayers,
        float traceScale = 1f)
    {
        if (mesh == null)
            throw new ArgumentNullException(nameof(mesh));
        if (meshTransform == null)
            throw new ArgumentNullException(nameof(meshTransform));
        if (!mesh.isReadable)
            throw new InvalidOperationException("Mesh must be readable: " + mesh.name);
        if (!physicsScene.IsValid())
            throw new ArgumentException("Physics scene is not valid.", nameof(physicsScene));

        Vector3[] positions = mesh.vertices;
        Vector3[] normals = mesh.normals;
        if (normals == null || normals.Length != positions.Length)
            throw new InvalidOperationException("Mesh normals do not match vertices: " + mesh.name);

        var result = new MaskSet(positions.Length);
        var hits = new RaycastHit[MaxTraceHits];
        float traceLength = TraceLengthUu * L2StaticMeshVertexLight.UuToMeters * Mathf.Max(traceScale, 0.0001f);

        for (int slot = 0; slot < SlotCount; slot++)
        {
            Vector3 incoming = GetIncomingDirection(slot);
            result.IncomingDirections[slot] = incoming;
            Vector3 directionToSun = -incoming;

            for (int vertex = 0; vertex < positions.Length; vertex++)
            {
                Vector3 worldNormal = meshTransform.TransformDirection(normals[vertex]).normalized;

                // Illuminate rejects a vertex when dot(safeDirection, normal) > 0.
                if (Vector3.Dot(incoming, worldNormal) > 0f)
                    continue;

                Vector3 worldPosition = meshTransform.TransformPoint(positions[vertex]);
                if (!IsOccluded(
                        physicsScene,
                        worldPosition,
                        directionToSun,
                        traceLength,
                        collisionMask,
                        ownerColliders,
                        hits))
                {
                    result.SetEnabled(slot, vertex);
                }
            }
        }

        return result;
    }
}
