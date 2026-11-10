using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemDropPresentationService
{
    void HideCoinDropMeshUntilPile(ItemEntity item, int itemObjectId)
    {
        SetDropMeshActive(item, false);
        CancelCoinMeshReveal(itemObjectId);
        _coinRevealByItemId[itemObjectId] = new CoinMeshRevealState
        {
            Item = item,
            RevealTime = Time.time + ItemDropPresentationIds.CoinDropMeshRevealDelaySeconds
        };
    }

    void TickCoinMeshReveals()
    {
        if (_coinRevealByItemId.Count == 0)
            return;

        float now = Time.time;
        _expiredCoinRevealIds.Clear();
        foreach (KeyValuePair<int, CoinMeshRevealState> pair in _coinRevealByItemId)
        {
            if (pair.Value.RevealTime <= now)
                _expiredCoinRevealIds.Add(pair.Key);
        }

        for (int i = 0; i < _expiredCoinRevealIds.Count; i++)
            RevealCoinDropMesh(_expiredCoinRevealIds[i]);
    }

    void RevealCoinDropMesh(int itemObjectId)
    {
        if (!_coinRevealByItemId.TryGetValue(itemObjectId, out CoinMeshRevealState state))
            return;

        _coinRevealByItemId.Remove(itemObjectId);
        if (state.Item == null)
            return;

        SetDropMeshActive(state.Item, true);
        _clickArea.Refresh(state.Item);
        ItemPickupMotion motion = state.Item.GetComponent<ItemPickupMotion>();
        if (motion != null)
            motion.BeginSpin(itemObjectId);
    }

    void CancelCoinMeshReveal(int itemObjectId)
    {
        _coinRevealByItemId.Remove(itemObjectId);
    }

    static void SetDropMeshActive(ItemEntity item, bool active)
    {
        if (item == null)
            return;
        Transform dropMesh = item.transform.Find("DropMesh");
        if (dropMesh != null)
            dropMesh.gameObject.SetActive(active);
    }

}
