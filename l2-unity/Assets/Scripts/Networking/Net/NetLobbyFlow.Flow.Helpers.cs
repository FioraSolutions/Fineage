using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetLobbyFlow
{
    void HandleLobby(PacketEnvelope envelope)
    {
        switch (envelope.Opcode)
        {
            case NetOpcode.HelloAck:
                break;
            case NetOpcode.AuthResult:
            {
                var result = NetCodec.DecodeAuthResult(envelope.Payload);
                _authResult?.Invoke(result);
                if (result.Ok)
                {
                    _ = RequestRoster();
                    break;
                }

                _awaitingLoginUi = false;
                string authLine = string.IsNullOrWhiteSpace(result.Message)
                    ? (string.IsNullOrWhiteSpace(result.Code) ? "Login failed." : result.Code)
                    : result.Message;
                ShowLoginFailure(authLine);
                break;
            }
            case NetOpcode.CharacterRoster:
            {
                var roster = NetCodec.DecodeCharacterRoster(envelope.Payload);
                _awaitingLoginUi = false;
                ApplyRosterUi(roster);
                _rosterReceived?.Invoke(roster);
                break;
            }
            case NetOpcode.CreateResult:
            {
                var created = NetCodec.DecodeCreateResult(envelope.Payload);
                if (!created.Ok)
                {
                    Debug.LogWarning($"[Net] create failed {created.Code}: {created.Message}");
                    if (LoginPacketServices.CharCreate != null)
                        LoginPacketServices.CharCreate.SetlabelError(
                            LobbyCharacterText.ErrorMessage(created.Code, created.Message));
                    break;
                }

                CharacterSummaryNet summary = created.Character;
                int newId = summary != null ? summary.CharacterId : 0;
                Debug.Log("[Net] character created id=" + newId);
                if (LoginPacketServices.CharCreate != null)
                    LoginPacketServices.CharCreate.HideWindow();
                _ = RequestRoster();
                break;
            }
            case NetOpcode.DeleteResult:
            {
                var deleted = NetCodec.DecodeDeleteResult(envelope.Payload);
                if (!deleted.Ok)
                    Debug.LogWarning($"[Net] delete/restore failed {deleted.Code}: {deleted.Message}");
                _deleteResultReceived?.Invoke(deleted);
                break;
            }
            case NetOpcode.Handoff:
            {
                var handoff = NetCodec.DecodeHandoff(envelope.Payload);
                _handoff = handoff;
                _handoffReceived?.Invoke(handoff);
                _ = EnterZoneWithHandoff(handoff);
                break;
            }
            case NetOpcode.Reject:
            {
                var reject = NetCodec.DecodeReject(envelope.Payload);
                Debug.LogWarning($"[Net] lobby reject {reject.Code}: {reject.Message}");
                if (_awaitingLoginUi)
                {
                    _awaitingLoginUi = false;
                    string line = NetRejectLines.Resolve(reject.Code, reject.Message);
                    if (string.IsNullOrEmpty(line))
                        line = string.IsNullOrWhiteSpace(reject.Message) ? "Login failed." : reject.Message;
                    ShowLoginFailure(line);
                }
                break;
            }
        }
    }

    async Task SendHelloAsync(NetTransport transport, string clientName)
    {
        await transport.SendAsync(
            NetOpcode.Hello,
            NetCodec.EncodeHello(new HelloMsg
            {
                ClientName = clientName,
                FeatureFlags = ProtocolCapabilities.FeatureSession
                    | ProtocolCapabilities.FeatureLobby
                    | ProtocolCapabilities.FeatureWorldPresence
            }),
            _nextSeq());
    }
}
