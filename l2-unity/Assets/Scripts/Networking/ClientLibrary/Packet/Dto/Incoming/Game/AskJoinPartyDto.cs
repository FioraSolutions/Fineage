using System;

public class AskJoinPartyDto : IWireDto
{
    private string _requestorName;
    private PartyDistributionType _partyDistributionType;
    private int _requestorObjectId;

    public string RequestorName => _requestorName;
    public PartyDistributionType DistributionType => _partyDistributionType;
    /// <summary>Zone V2 inviter object id; zero on Interlude wire.</summary>
    public int RequestorObjectId => _requestorObjectId;

    /// <summary>Build from Zone V2 <see cref="ZoneV2MsgType.PartyAskJoin"/> (1027).</summary>
    public static AskJoinPartyDto FromZoneV2(int requestorObjectId, string requestorName, int itemDistribution)
    {
        var dto = new AskJoinPartyDto();
        dto._requestorObjectId = requestorObjectId;
        dto._requestorName = requestorName ?? "";
        dto._partyDistributionType = PartyDistributionTypeExtensions.FindById(itemDistribution) ??
                                       PartyDistributionType.FindersKeepers;
        return dto;
    }

    

    public void ReadFrom(PacketReader reader)
    {
        try
        {
            // Читаем packetId (уже должен быть прочитан в базовом классе для идентификации пакета)
            _requestorName = reader.ReadOtherS();
            int distributionTypeId = reader.ReadI();

            // Конвертируем ID в enum
            _partyDistributionType = PartyDistributionTypeExtensions.FindById(distributionTypeId) ??
                                   PartyDistributionType.FindersKeepers;
        }
        catch (Exception ex)
        {
            // Debug.LogError($"[AskJoinParty] Parse error: {ex.Message}");
            _requestorName = string.Empty;
            _partyDistributionType = PartyDistributionType.FindersKeepers;
        }
    }

    public override string ToString()
    {
        return $"[AskJoinParty] Requestor: {_requestorName}, DistributionType: {_partyDistributionType}";
    }
}