using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class InterludeLoginProof : MonoBehaviour
{
    public static void Arm(VisualElement documentRoot)
    {
        if (!IsArmed || documentRoot == null)
            return;

        var host = new GameObject("InterludeLoginProof");
        var proof = host.AddComponent<InterludeLoginProof>();
        proof._root = documentRoot;
        proof.StartCoroutine(proof.Capture());
    }
}
