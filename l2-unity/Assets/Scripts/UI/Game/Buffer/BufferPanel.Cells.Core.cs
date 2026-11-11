public partial class BufferPanel
{
    public void RefreshPenalty(EtcStatusUpdateDto etcStatusUpdatePacket)
    {
        _deathPenalty = etcStatusUpdatePacket.DeathPenalty;
        _weightPenalty = etcStatusUpdatePacket.WeightPenalty;
        _weaponPenalty = etcStatusUpdatePacket.WeaponPenalty;

        AddDataCell(_deathPenalty[0], _deathPenalty[1]);
        AddDataCell(_weightPenalty[0], _weightPenalty[1]);
        AddDataCell(_weaponPenalty[0], _weaponPenalty[1]);

        DeleteDataCell(_deathPenalty[0], _deathPenalty[1]);
        DeleteDataCell(_weightPenalty[0], _weightPenalty[1]);
        DeleteDataCell(_weaponPenalty[0], _weaponPenalty[1]);
    }

    public void AddDataCell(int skillId, int skillLevel)
    {
        if (skillLevel == 0) return;
        if (_content.style.opacity == 0) _content.style.opacity = 1;
        if (_filterData.IsContain(skillId)) return;

        DataCell data = _filterData.GetEmptyCell();
        data.RefreshData(skillId, true, skillLevel);
        data.ShowCell(true);
    }

    public void AddDataCellToTime(int skillId, int skillLevel, int time)
    {

        if (skillLevel == 0) return;

        Skillgrp skill = SkillGrps.GetSkill(skillId, skillLevel);
        if (skill == null)
            Debug.LogWarning($"BufferPanel.AddDataCellToTime: skillgrp missing skillId={skillId} level={skillLevel} — still showing tray cell");

        if (_content.style.opacity == 0) _content.style.opacity = 1;
        if (_filterData.IsContain(skillId))
        {
            DataCell existing = _filterData.FilterBySkillId(skillId);
            if (existing != null && time > 0)
            {
                existing.SetActiveTime(time);
                _bLink.StartBlinking(existing);
            }

            return;
        }

        RebindCellElseOnlyPenalty();
        DataCell data = _filterData.GetEmptyCell();
        data.RefreshData(skillId, true, skillLevel);
        data.ShowCell(true);
        data.SetActiveTime(time);
        _bLink.StartBlinking(data);
    }

    public void RemoveAllEffects()
    {
        List<DataCell> list = _filterData.GetListActiveAndBusy();
        ResetList(list);
        // Stale blink entries must go too — otherwise the next FullEffectSnapshot
        // re-adds into the same slots and the blink dict throws / aborts the rebuild.
        _bLink?.ClearAllBlinking();
        HideContentElseNoElements();
    }

    private void HideContentElseNoElements()
    {
        if (!_filterData.HasElements())
        {
            _content.style.opacity = 0;
        }
    }

    private void ResetList(List<DataCell> list)
    {
        if (list != null && list.Count > 0)
        {
            foreach (DataCell cell in list)
            {
                _bLink?.StopBlinking(cell.GetPosition());
                cell.ResetData();
                cell.ShowCell(false);
            }
        }
    }
}
