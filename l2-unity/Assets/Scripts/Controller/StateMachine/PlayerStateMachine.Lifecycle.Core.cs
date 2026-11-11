using System;
using UnityEngine;
using System.Collections.Generic;

public partial class PlayerStateMachine : MonoBehaviour
{
    public int GetObjectId()
    {
        if (Player == null) return -1;
        return Player.Identity.Id;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
         
        }
        else if (TryClaimInstanceFromPlayerEntity())
        {
            // Prefer PlayerStateMachine on the spawned player over the scene stub.
        }
        else
        {
            Destroy(this);
            return;
        }

        InitializeState();
        InitializeIntention();
        PlayerStatePacketServices.Bind(playerState: this);

        this.enabled = false;
    }

    bool TryClaimInstanceFromPlayerEntity()
    {
        if (GetComponentInParent<PlayerEntity>() == null)
            return false;

        PlayerStateMachine previous = _instance;
        _instance = this;
        PlayerStatePacketServices.Bind(playerState: this);
        if (previous != null && previous != this && previous.GetComponentInParent<PlayerEntity>() == null)
            Destroy(previous);
        return true;
    }

    public static void AdoptFromPlayer(PlayerEntity player)
    {
        if (player == null)
            return;

        PlayerStateMachine onPlayer = player.GetComponent<PlayerStateMachine>();
        if (onPlayer == null)
            return;

        if (_instance != null && _instance != onPlayer && _instance.GetComponentInParent<PlayerEntity>() == null)
            Destroy(_instance);

        _instance = onPlayer;
        onPlayer.enabled = true;
        PlayerStatePacketServices.Bind(playerState: onPlayer);
        if (DomainPacketServices.LocalPlayer != null)
            onPlayer.Player = DomainPacketServices.LocalPlayer;
    }
}
