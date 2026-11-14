using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using VContainer;

public partial class StatusWindow
{
    private void SendSelfTargetAction(PlayerEntity player)
    {
        if (player.Identity == null || !ZoneCommands.IsConnected)
        {
            return;
        }

        var position = player.transform.position;
        var l2jpos = VectorUtils.ConvertPosUnityToL2j(position);

        ZoneCommands.Send(new ClickActionCommand(
            player.Identity.Id,
            (int)l2jpos.x,
            (int)l2jpos.y,
            (int)l2jpos.z,
            0));
    }

    void FixedUpdate()
    {
        if(DomainPacketServices.LocalPlayer == null) { 
            return; 
        }

        if(!(DomainPacketServices.LocalPlayer.Status is PlayerStatus)) {
            // Debug.LogWarning("Player status is not of type playerstatus");
            return;
        }

        PlayerStatus status = (PlayerStatus)DomainPacketServices.LocalPlayer.Status;
        PlayerStats stats = (PlayerStats)DomainPacketServices.LocalPlayer.Stats;

        if (_levelLabel != null) {
            _levelLabel.text = stats.Level.ToString();
        }

        if(_nameLabel != null) {
            _nameLabel.text = DomainPacketServices.LocalPlayer.Identity.Name;
        }

        if(_CPTextLabel != null) {
            int cpMax = PlayerResourceCaps.EffectiveMax(stats.MaxCp, status.Cp);
            int cpCur = PlayerResourceCaps.DisplayCurrent(status.Cp, cpMax);
            _CPTextLabel.text = cpCur + "/" + cpMax;
        }

        if(_HPTextLabel != null) {
            int hpMax = PlayerResourceCaps.EffectiveMax(stats.MaxHp, status.GetHp());
            int hpCur = PlayerResourceCaps.DisplayCurrent(status.GetHp(), hpMax);
            _HPTextLabel.text = hpCur + "/" + hpMax;
        }

        if(_MPTextLabel != null) {
            int mpMax = PlayerResourceCaps.EffectiveMax(stats.MaxMp, status.GetMp());
            int mpCur = PlayerResourceCaps.DisplayCurrent(status.GetMp(), mpMax);
            _MPTextLabel.text = mpCur + "/" + mpMax;
        }

        L2ResourceBarView.SetFill(
            _CPBar,
            _CPBarBG,
            PlayerResourceCaps.BarFillRatio(status.Cp, stats.MaxCp));
        L2ResourceBarView.SetFill(
            _HPBar,
            _HPBarBG,
            PlayerResourceCaps.BarFillRatio(status.GetHp(), stats.MaxHp));
        L2ResourceBarView.SetFill(
            _MPBar,
            _MPBarBG,
            PlayerResourceCaps.BarFillRatio(status.GetMp(), stats.MaxMp));

        float expRatio = LevelExpRatio(stats);
        if (_EXPTextLabel != null) {
            _EXPTextLabel.text = (expRatio * 100f).ToString("0.00") + "%";
        }
        L2ResourceBarView.SetFill(_EXPBar, _EXPBarBG, expRatio);
    }

    void PaintStatusChrome()
    {
        PaintBar(_CPBarBG, "Data/UI/Status/Interlude/Status_Interlude_Ps_CpBar_Back");
        PaintBar(_CPBar, "Data/UI/Status/Interlude/Status_Interlude_Ps_CpBar");
        PaintBar(_HPBarBG, "Data/UI/Status/Interlude/Status_Interlude_Ps_HpBar_Back");
        PaintBar(_HPBar, "Data/UI/Status/Interlude/Status_Interlude_Ps_HpBar");
        PaintBar(_MPBarBG, "Data/UI/Status/Interlude/Status_Interlude_Ps_MpBar_Back");
        PaintBar(_MPBar, "Data/UI/Status/Interlude/Status_Interlude_Ps_MpBar");
        PaintBar(_EXPBarBG, "Data/UI/Status/Interlude/Status_Interlude_Ps_ExpBar_Back");
        PaintBar(_EXPBar, "Data/UI/Status/Interlude/Status_Interlude_Ps_ExpBar");

        VisualElement level = GetElementByClass("level-container");
        if (level != null)
        {
            Stamp(level, "Data/UI/Status/ps_levelback");
            level.style.unitySliceLeft = 6;
            level.style.unitySliceRight = 6;
            level.style.unitySliceTop = 6;
            level.style.unitySliceBottom = 6;
            level.style.unitySliceScale = 1f;
            level.style.width = 18;
            level.style.height = 16;
            level.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        }

        PaintSideFrame();

        VisualElement grip = GetElementByClass("hor-resize-handle");
        if (grip != null)
        {
            Stamp(grip, "Data/UI/Frame/Frame_DF_Ver/ps_sizecontrol2");
            grip.style.unitySliceLeft = 0;
            grip.style.unitySliceRight = 0;
            grip.style.unitySliceTop = 0;
            grip.style.unitySliceBottom = 0;
            grip.style.width = 7;
            grip.style.height = 16;
            grip.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        }
    }

    void PaintSideFrame()
    {
        VisualElement frame = GetElementById("Frame_DF_Ver");
        if (frame == null)
            return;

        VisualElement column = frame.Q<GroupBox>("Frame_DF_Ver");
        if (column == null || column.childCount < 4)
            return;

        Stamp(column[0], "Data/UI/Frame/Frame_DF_Ver/Frame_DF_Ver_Small_Top");
        Stamp(column[1], "Data/UI/Frame/Frame_DF_Ver/Frame_DF_Ver_Small_Center");
        Stamp(column[2], "Data/UI/Frame/Frame_DF_Ver/Frame_DF_Ver_Small_Center");
        Stamp(column[3], "Data/UI/Frame/Frame_DF_Ver/Frame_DF_Ver_Small_Bottom");
        for (int i = 0; i < column.childCount; i++)
            column[i].style.unityBackgroundScaleMode = ScaleMode.StretchToFill;

        VisualElement edge = GetElementById("SizeControl");
        if (edge != null)
        {
            Stamp(edge, "Data/UI/Frame/Frame_DF_Ver/ps_sizecontrol1");
            edge.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        }
    }

    static void PaintBar(VisualElement bar, string resourcesPath)
    {
        if (bar == null)
            return;

        Texture2D texture = LoadUiTexture(resourcesPath);
        if (texture == null)
            return;

        List<VisualElement> pieces = bar.Query<VisualElement>(className: "center").ToList();
        if (pieces.Count == 0)
            pieces.Add(bar);

        for (int i = 0; i < pieces.Count; i++)
        {
            VisualElement piece = pieces[i];
            piece.style.backgroundImage = new StyleBackground(texture);
            piece.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        }
    }

    static void Stamp(VisualElement element, string resourcesPath)
    {
        Texture2D texture = LoadUiTexture(resourcesPath);
        if (element == null || texture == null)
            return;

        element.style.backgroundImage = new StyleBackground(texture);
    }

    static Texture2D LoadUiTexture(string resourcesPath)
    {
        if (IconPacketServices.Icons != null)
        {
            Texture2D loaded = IconPacketServices.Icons.LoadTextureOtherSources(resourcesPath);
            if (loaded != null && loaded.name != "NOIMAGE")
                return loaded;
        }

        return Resources.Load<Texture2D>(resourcesPath);
    }

    static float LevelExpRatio(PlayerStats stats)
    {
        int lvl = stats.Level;
        long cur = LevelServer.GetExp(lvl);
        long next = LevelServer.GetExp(lvl + 1);
        if (cur < 0 || next <= cur)
            return 0f;
        double r = (stats.Exp - cur) / (double)(next - cur);
        if (double.IsNaN(r)) return 0f;
        return Mathf.Clamp01((float)r);
    }
}
