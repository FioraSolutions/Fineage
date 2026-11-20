using System;
using Fineage.Net.Contract;

/// <summary>
/// Data-driven apply disposition per zone opcode. Consumers query the table — no scattered opcode if-chains.
/// </summary>
public static class NetPacketApplyPolicy
{
    public enum Disposition
    {
        /// <summary>Never dropped; FIFO preserve queue (entity lifecycle and state).</summary>
        Preserve = 0,
        /// <summary>Latest-wins per coalesce key; superseded pending work is replaced, not dropped.</summary>
        Coalesce = 1,
        /// <summary>May drop oldest entry when the discardable backlog exceeds the soft cap.</summary>
        Discardable = 2,
    }

    static readonly Disposition[] ByOpcode = new Disposition[ushort.MaxValue + 1];
    static readonly Func<object, long>[] CoalesceKeyByOpcode = new Func<object, long>[ushort.MaxValue + 1];

    static NetPacketApplyPolicy()
    {
        // Safe default: state-creating and lifecycle traffic is never dropped.
        for (int i = 0; i < ByOpcode.Length; i++)
            ByOpcode[i] = Disposition.Preserve;

        RegisterCoalesce(NetOpcode.MoveState, EntityKey<MoveStateMsg>(NetOpcode.MoveState, m => m.EntityValue));
        RegisterCoalesce(NetOpcode.PositionCorrection, EntityKey<PositionCorrectionMsg>(NetOpcode.PositionCorrection, m => m.EntityValue));
        RegisterCoalesce(NetOpcode.StatusUpdate, EntityKey<StatusUpdateNetMsg>(NetOpcode.StatusUpdate, m => m.EntityValue));
        RegisterCoalesce(NetOpcode.ChangeMoveType, EntityKey<ChangeMoveTypeNetMsg>(NetOpcode.ChangeMoveType, m => m.EntityValue));
        RegisterCoalesce(NetOpcode.AttackState, EntityKey<AttackStateMsg>(NetOpcode.AttackState, m => m.AttackerValue));
        RegisterCoalesce(NetOpcode.PartyMemberUpdate, EntityKey<PartyMemberUpdateNetMsg>(NetOpcode.PartyMemberUpdate, m => (ulong)m.ObjectId));
        RegisterCoalesceKey(NetOpcode.PartyMemberPosition, PartyMemberPositionKey);

        RegisterDiscardable(NetOpcode.CreatureSay);
        RegisterDiscardable(NetOpcode.SystemMessage);
        RegisterDiscardable(NetOpcode.PlaySound);
    }

    public static Disposition GetDisposition(ushort opcode) => ByOpcode[opcode];

    /// <summary>Non-zero when <see cref="GetDisposition"/> is <see cref="Disposition.Coalesce"/> and the body supports a key.</summary>
    public static long GetCoalesceKey(ushort opcode, object body)
    {
        if (body == null)
            return 0;
        Func<object, long> fn = CoalesceKeyByOpcode[opcode];
        return fn == null ? 0 : fn(body);
    }

    public static string OpcodeLabel(ushort opcode) => $"0x{opcode:X4}";

    static void RegisterCoalesceKey(ushort opcode, Func<object, long> keyFromBody)
    {
        ByOpcode[opcode] = Disposition.Coalesce;
        CoalesceKeyByOpcode[opcode] = keyFromBody;
    }

    static void RegisterCoalesce<T>(ushort opcode, Func<T, long> keyFromBody) =>
        RegisterCoalesceKey(opcode, body => body is T typed ? keyFromBody(typed) : 0);

    static void RegisterDiscardable(ushort opcode) => ByOpcode[opcode] = Disposition.Discardable;

    static Func<T, long> EntityKey<T>(ushort opcode, Func<T, ulong> entityValue) where T : class =>
        m => PackCoalesceKey(opcode, entityValue(m));

    static long PartyMemberPositionKey(object body)
    {
        if (body is not PartyMemberPositionNetMsg msg || msg.Members == null)
            return 0;
        // One coalesce slot per periodic fanout (latest positions for the whole party).
        return PackCoalesceKey(NetOpcode.PartyMemberPosition, 0UL);
    }

    static long PackCoalesceKey(ushort opcode, ulong entityValue) =>
        ((long)opcode << 32) | (long)(uint)entityValue;
}