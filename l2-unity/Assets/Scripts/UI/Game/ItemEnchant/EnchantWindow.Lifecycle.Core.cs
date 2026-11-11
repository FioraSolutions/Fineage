using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using VContainer;

public partial class EnchantWindow : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _loadingAnimWaitPng = new List<Texture2D>();
            _loadingAnimSuccessPng = new List<Texture2D>();
            _loadingAnimFailPng = new List<Texture2D>();
            CacheTexture();
            WireItemEnchantPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireItemEnchantPacketServices();
    }

    private void WireItemEnchantPacketServices()
    {
        ItemEnchantPacketServices.Bind(enchant: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ItemEnchant/EnchantWindow");
        _inventorySlotTemplate = LoadAsset("Data/UI/_Elements/Template/SlotEnchant");
        _inventorySlotChoiceTemplate = LoadAsset("Data/UI/_Elements/Template/SlotEnchantChoice");
    }
}
