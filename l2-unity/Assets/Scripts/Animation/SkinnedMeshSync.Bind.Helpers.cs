using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SkinnedMeshSync : MonoBehaviour
{
    IEnumerator ProcessWaitingObjects()
    {
        _isProcessing = true;
        while (_waitingObjects.Count > 0)
        {
            if (!gameObject.activeInHierarchy)
                break;

            GameObject current = _waitingObjects.Dequeue();
            yield return StartCoroutine(AttachAndSync(current));
            yield return new WaitForSeconds(0.1f);
        }
        _isProcessing = false;
    }

    IEnumerator AttachAndSync(GameObject newObject)
    {
        if (transform.parent == null || newObject == null)
            yield break;

        newObject.transform.SetParent(transform.parent, false);
        yield return null;
        SyncMesh();
        ShowRenderers(newObject);
    }

    void HideRenderers(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = false;
            _temporarilyHiddenRenderers.Add(renderers[i]);
        }
    }

    void ShowRenderers(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (_temporarilyHiddenRenderers.Contains(renderer))
                renderer.enabled = true;
        }
    }
}
