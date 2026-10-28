using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

public class ZoneV2StatusUpdateWireTests
{
    [Fact]
    public void Status_update_opcode_matches_backend_regen_fanout()
    {
        Assert.Equal(1010, (ushort)ZoneV2MsgType.StatusUpdate);
    }

    [Fact]
    public void Regen_tick_status_update_decodes_cur_and_max_hp_mp()
    {
        const int objectId = 1001;
        int[] ids = { 0x09, 0x0a, 0x0b, 0x0c };
        int[] values = { 82, 100, 44, 100 };

        byte[] body = ZoneV2Messages.EncodeStatusUpdate(objectId, ids, values);

        Assert.True(ZoneV2Messages.TryDecodeStatusUpdate(body, out ZoneV2StatusUpdate status));
        Assert.Equal(objectId, status.ObjectId);
        Assert.Equal(ids, status.AttributeIds);
        Assert.Equal(values, status.Values);
    }

    [Fact]
    public void Combat_wound_status_update_decodes_hp_only_batch()
    {
        byte[] body = ZoneV2Messages.EncodeStatusUpdate(
            20_003,
            new[] { 0x09, 0x0a },
            new[] { 64, 96 });

        Assert.True(ZoneV2Messages.TryDecodeStatusUpdate(body, out ZoneV2StatusUpdate status));
        Assert.Equal(20_003, status.ObjectId);
        Assert.Equal(new[] { 0x09, 0x0a }, status.AttributeIds);
        Assert.Equal(new[] { 64, 96 }, status.Values);
    }
}
