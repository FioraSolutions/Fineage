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
        /// <summary>Superseded pending work is merged or replaced per <see cref="CoalesceStrategy"/>.</summary>
        Coalesce = 1,
        /// <summary>May drop oldest entry when the discardable backlog exceeds the soft cap.</summary>
        Discardable = 2,
    }

    public enum CoalesceStrategy
    {
        /// <summary>Whole message is authoritative; keep the newest pending body.</summary>
        LatestWins = 0,
        /// <summary>Partial attribute rows (L2 StatusUpdate): union per attribute id, newest value wins per id.</summary>
        MergeAttributeMap = 1,
        /// <summary>Periodic party map positions with only changed members: union per object id.</summary>
        MergeKeyedRows = 2,
    }

    static readonly Disposition[] ByOpcode = new Disposition[ushort.MaxValue + 1];
    static readonly Func<object, long>[] CoalesceKeyByOpcode = new Func<object, long>[ushort.MaxValue + 1];
    static readonly CoalesceStrategy[] CoalesceStrategyByOpcode = new CoalesceStrategy[ushort.MaxValue + 1];

    static NetPacketApplyPolicy()
    {
        for (int i = 0; i < ByOpcode.Length; i++)
            ByOpcode[i] = Disposition.Preserve;

        RegisterCoalesceLatest(NetOpcode.MoveState, EntityKey<MoveStateMsg>(NetOpcode.MoveState, m => m.EntityValue));
        RegisterCoalesceLatest(NetOpcode.PositionCorrection, EntityKey<PositionCorrectionMsg>(NetOpcode.PositionCorrection, m => m.EntityValue));
        RegisterCoalesceMerge(NetOpcode.StatusUpdate, CoalesceStrategy.MergeAttributeMap, EntityKey<StatusUpdateNetMsg>(NetOpcode.StatusUpdate, m => m.EntityValue));
        RegisterCoalesceLatest(NetOpcode.ChangeMoveType, EntityKey<ChangeMoveTypeNetMsg>(NetOpcode.ChangeMoveType, m => m.EntityValue));
        RegisterCoalesceLatest(NetOpcode.AttackState, EntityKey<AttackStateMsg>(NetOpcode.AttackState, m => m.AttackerValue));
        RegisterCoalesceLatest(NetOpcode.PartyMemberUpdate, EntityKey<PartyMemberUpdateNetMsg>(NetOpcode.PartyMemberUpdate, m => (ulong)m.ObjectId));
        RegisterCoalesce(NetOpcode.PartyMemberPosition, CoalesceStrategy.MergeKeyedRows, PartyMemberPositionKey);

        RegisterDiscardable(NetOpcode.PlaySound);
    }

    public static Disposition GetDisposition(ushort opcode) => ByOpcode[opcode];

    public static CoalesceStrategy GetCoalesceStrategy(ushort opcode) => CoalesceStrategyByOpcode[opcode];

    /// <summary>Non-zero when <see cref="GetDisposition"/> is <see cref="Disposition.Coalesce"/> and the body supports a key.</summary>
    public static long GetCoalesceKey(ushort opcode, object body)
    {
        if (body == null)
            return 0;
        Func<object, long> fn = CoalesceKeyByOpcode[opcode];
        return fn == null ? 0 : fn(body);
    }

    public static object MergeCoalesceBodies(ushort opcode, object accumulated, object incoming) =>
        NetPacketCoalesceMerge.Merge(opcode, accumulated, incoming);

    public static string OpcodeLabel(ushort opcode) => $"0x{opcode:X4}";

    static void RegisterCoalesce(ushort opcode, CoalesceStrategy strategy, Func<object, long> keyFromBody)
    {
        ByOpcode[opcode] = Disposition.Coalesce;
        CoalesceStrategyByOpcode[opcode] = strategy;
        CoalesceKeyByOpcode[opcode] = keyFromBody;
    }

    static void RegisterCoalesceLatest<T>(ushort opcode, Func<T, long> keyFromBody) =>
        RegisterCoalesceMerge(opcode, CoalesceStrategy.LatestWins, keyFromBody);

    static void RegisterCoalesceMerge<T>(ushort opcode, CoalesceStrategy strategy, Func<T, long> keyFromBody) =>
        RegisterCoalesce(opcode, strategy, body => body is T typed ? keyFromBody(typed) : 0);

    static void RegisterDiscardable(ushort opcode) => ByOpcode[opcode] = Disposition.Discardable;

    static Func<T, long> EntityKey<T>(ushort opcode, Func<T, ulong> entityValue) where T : class =>
        m => PackCoalesceKey(opcode, entityValue(m));

    static long PartyMemberPositionKey(object body)
    {
        if (body is not PartyMemberPositionNetMsg)
            return 0;
        return PackCoalesceKey(NetOpcode.PartyMemberPosition, 0UL);
    }

    static long PackCoalesceKey(ushort opcode, ulong entityValue) =>
        ((long)opcode << 32) | (long)(uint)entityValue;
}