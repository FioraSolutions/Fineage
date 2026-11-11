using System.Collections.Generic;
using System;

public sealed class StatusUpdateDto : INetworkModel
{
    public int ObjectId;
    public List<StatusUpdate.Attribute> Attributes;
}
