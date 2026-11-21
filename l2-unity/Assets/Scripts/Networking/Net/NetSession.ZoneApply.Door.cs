using Fineage.Net.Contract;

public sealed partial class NetSession
{
    internal void ApplyDoorStatus(DoorStatusNetMsg msg) => NetDoorApply.Apply(msg);
}