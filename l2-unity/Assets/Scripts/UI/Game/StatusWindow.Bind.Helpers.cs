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
            grip.style.width = 7;
            grip.style.height = 16;
        }
    }

    void PaintSideFrame()
    {
        L2InterludeHudVerticalFrame.TryPaintVerticalColumn(_windowEle ?? _root, nameof(StatusWindow));

        VisualElement grip = GetElementById("SizeControlHandle");
        if (grip != null)
            L2InterludeHudVerticalFrame.StampHorizontalResize(grip, nameof(StatusWindow));
    }

    static void PaintBar(VisualElement bar, string resourcesPath)
    {
        if (bar == null)
            return;

        Texture2D texture = LoadUiTexture(resourcesPath, nameof(StatusWindow));
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
        Texture2D texture = LoadUiTexture(resourcesPath, nameof(StatusWindow));
        if (element == null || texture == null)
            return;

        element.style.backgroundImage = new StyleBackground(texture);
    }

    static Texture2D LoadUiTexture(string resourcesPath, string context)
    {
        Texture2D texture = null;
        if (IconPacketServices.Icons != null)
        {
            Texture2D loaded = IconPacketServices.Icons.LoadTextureOtherSources(resourcesPath);
            if (loaded != null && loaded.name != "NOIMAGE")
                texture = loaded;
        }

        texture ??= Resources.Load<Texture2D>(resourcesPath);
        if (texture == null)
        {
            Debug.LogError(
                "[StatusWindow] missing HUD texture path='" + resourcesPath +
                "' context='" + context + "'. Import from ref/l2-interlude-client L2UI_CH3.");
        }

        return texture;
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
