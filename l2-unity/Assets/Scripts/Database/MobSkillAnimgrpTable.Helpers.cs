using System;

public sealed partial class MobSkillAnimgrpTable
{
    public bool TryGetSequence(int npcId, int skillId, out string sequence)
    {
        return _sequences.TryGetValue(Key(npcId, skillId), out sequence);
    }

    private static long Key(int npcId, int skillId)
    {
        return ((long)(uint)npcId << 32) | (uint)skillId;
    }
}
