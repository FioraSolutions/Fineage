using VContainer;

public sealed partial class GameRuntime
{
    public void BindApply(WorldPacketApply worldApply, MessagePacketApply messageApply)
    {
        if (worldApply != null)
            WorldApply = worldApply;
        if (messageApply != null)
            MessageApply = messageApply;
    }
}
