using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillListWindow : L2PopupWindow
{
    public bool IsWindowContain(Vector2 vector2)
    {
        return  _windowEle.worldBound.Contains(vector2);
    }

    public void EmbedInto(VisualElement host)
    {
        if (host == null)
            return;
        if (_windowEle == null)
            ShowWindow();
        if (_windowEle == null)
            return;
        _windowEle.style.display = DisplayStyle.None;
        VisualElement body = GetElementById("content") ?? _windowEle;
        body.AddToClassList("skills-panel");
        body.AddToClassList("skills-embed");
        VisualElement cancelRow = body.Q("Buttons");
        if (cancelRow != null)
            cancelRow.style.display = DisplayStyle.None;
        if (body.parent != host)
        {
            body.RemoveFromHierarchy();
            host.Add(body);
        }
        body.style.position = Position.Relative;
        body.style.left = 0;
        body.style.top = 0;
        body.style.right = StyleKeyword.Auto;
        body.style.bottom = StyleKeyword.Auto;
        body.style.width = Length.Percent(100);
        body.style.height = Length.Percent(100);
        body.style.marginTop = 0;
        body.style.marginRight = 0;
        body.style.marginBottom = 0;
        body.style.marginLeft = 0;
        body.style.paddingTop = 2;
        body.style.paddingRight = 4;
        body.style.paddingBottom = 2;
        body.style.paddingLeft = 4;
        host.style.display = DisplayStyle.Flex;
        AudioPacketServices.Audio.PlayUISound("window_open");
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SkillLearn/SkillListWindow");
        _tabTemplate = LoadAsset(L2InventoryUiAssets.Tab);
        _tabHeaderTemplate = LoadAsset("Data/UI/_Elements/Game/SkillLearn/SkillTabHeader");

        _templateActiveSkill = LoadAsset("Data/UI/_Elements/Game/SkillLearn/TabActive");
        _templatePassiveSkill = LoadAsset("Data/UI/_Elements/Game/SkillLearn/TabPassive");
        _templateLearnSkill = LoadAsset("Data/UI/_Elements/Game/SkillLearn/TabLearn");

        //template panels 8x1 skills
        _templateBoxPanel = LoadAsset("Data/UI/_Elements/Template/Skills/SkillBoxRow");
        _templateSkillsRow8x1 = LoadAsset("Data/UI/_Elements/Template/Skills/SkillPanels/SkillsRow8x1");
        _templateSlotSkill = LoadAsset("Data/UI/_Elements/Template/Skills/SkillPanels/SlotSkill");
    }
}
