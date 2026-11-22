#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using System;

public partial class L2BrushBuilder
{
    static Material FindMaterial(string texture, string brushName) {
        L2BrushFaceClass.Face face = L2BrushFaceClass.Classify(texture);
        Material material = L2BrushFaceClass.LoadMaterial(face);
        if (material != null) {
            return material;
        }
        Debug.LogError(
            "[Brushes] material missing " + face.Format()
            + " ref=" + texture
            + " brush=" + brushName);
        return null;
    }

    static GameObject Build(Brush[] brushes) {
        GameObject brushContainer = new GameObject("Brushes");

        foreach(Brush b in brushes) {
            if(b.position == Vector3.zero) {
                // Debug.LogWarning(b.name + " position is null");
                continue;
            }
            if(b.polyFlags != null) {
                List<string> polyFlags = new List<string>(b.polyFlags);
                if (polyFlags.Contains("PF_Invisible")) {
                    continue;
                }
                if (polyFlags.Contains("PF_NotSolid")) {
                    continue;
                }
            }

            GameObject brush = new GameObject(b.name);
            brush.transform.parent = brushContainer.transform;
            brush.transform.position = VectorUtils.ConvertPosToUnity(b.position) - VectorUtils.ConvertPosToUnity(b.prePivot);

            // Debug.Log(b.name);
            Model model = b.model;
            Poly poly = model.poly;

            for(int i = 0; i < poly.polyData.Length; i++) {
                PolyData polyData = poly.polyData[i];

                // Adjust verticles
                for(int p = 0; p < polyData.vertices.Length; p++) {
                    polyData.vertices[p] = VectorUtils.ConvertPosToUnity(polyData.vertices[p]);
                }

                List<string> pPolyFlags = new List<string>(polyData.polyFlags);

                // PF_Unlit is a real face (orc cave roof). Only invisible and non-solid faces are holes.
                if(pPolyFlags.Contains("PF_Invisible") || pPolyFlags.Contains("PF_NotSolid")) {
                    continue;
                }
                
                if(b.csgOper == "CSG_Subtract") {
                    // Only draw bottom face
                    // if (i != poly.polyData.Length - 1) {
                    //continue;
                    //}
                }

                //GameObject mesh = createMesh(b.csgOper, polyData);

                GameObject mesh = createProbuilderMesh(b.csgOper, polyData, i);
                mesh.transform.parent = brush.transform;
                mesh.transform.localPosition = Vector3.zero;
            }
        }

        return brushContainer;
    }
}
#endif
