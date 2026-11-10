using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace Fineage.Net.Contract
{
    public static partial class NetCodec
    {
        public static byte[] EncodeHello(HelloMsg m) => Write(w =>
        {
            w.WriteByte(m.Major);
            w.WriteByte(m.Minor);
            w.WriteString(m.ClientName);
            w.WriteUInt32(m.FeatureFlags);
        });

        public static HelloMsg DecodeHello(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new HelloMsg
            {
                Major = r.ReadByte(),
                Minor = r.ReadByte(),
                ClientName = r.ReadString(),
                FeatureFlags = r.ReadUInt32()
            };
        }

        public static byte[] EncodeHelloAck(HelloAckMsg m) => Write(w =>
        {
            w.WriteByte(m.Major);
            w.WriteByte(m.Minor);
            w.WriteUInt32(m.FeatureFlags);
            w.WriteString(m.ServerName);
        });

        public static HelloAckMsg DecodeHelloAck(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new HelloAckMsg
            {
                Major = r.ReadByte(),
                Minor = r.ReadByte(),
                FeatureFlags = r.ReadUInt32(),
                ServerName = r.ReadString()
            };
        }

        public static byte[] EncodeReject(RejectMsg m) => Write(w =>
        {
            w.WriteString(m.Code);
            w.WriteString(m.Message);
        });

        public static RejectMsg DecodeReject(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new RejectMsg { Code = r.ReadString(), Message = r.ReadString() };
        }

        public static byte[] EncodeAuth(AuthMsg m) => Write(w =>
        {
            w.WriteString(m.Account);
            w.WriteString(m.Password);
            w.WriteByte(m.RegisterIfMissing ? (byte)1 : (byte)0);
        });

        public static AuthMsg DecodeAuth(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new AuthMsg
            {
                Account = r.ReadString(),
                Password = r.ReadString(),
                RegisterIfMissing = r.ReadByte() != 0
            };
        }

        public static byte[] EncodeAuthResult(AuthResultMsg m) => Write(w =>
        {
            w.WriteByte(m.Ok ? (byte)1 : (byte)0);
            w.WriteString(m.Code);
            w.WriteString(m.Message);
            w.WriteString(m.Account);
        });

        public static AuthResultMsg DecodeAuthResult(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new AuthResultMsg
            {
                Ok = r.ReadByte() != 0,
                Code = r.ReadString(),
                Message = r.ReadString(),
                Account = r.ReadString()
            };
        }

        public static byte[] EncodeListCharacters(ListCharactersMsg _) => Array.Empty<byte>();

        public static ListCharactersMsg DecodeListCharacters(ReadOnlySpan<byte> _) => new();

        public static byte[] EncodeCharacterRoster(CharacterRosterMsg m) => Write(w =>
        {
            w.WriteUInt16((ushort)m.Characters.Length);
            foreach (var c in m.Characters)
                WriteSummary(w, c);
        });

        public static CharacterRosterMsg DecodeCharacterRoster(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            int n = r.ReadUInt16();
            var rows = new CharacterSummaryNet[n];
            for (int i = 0; i < n; i++)
                rows[i] = ReadSummary(ref r);
            return new CharacterRosterMsg { Characters = rows };
        }

        public static byte[] EncodeCreateCharacter(CreateCharacterMsg m) => Write(w =>
        {
            w.WriteString(m.Name);
            w.WriteInt32(m.ClassId);
            w.WriteInt32(m.Race);
            w.WriteInt32(m.Sex);
            w.WriteInt32(m.HairStyle);
            w.WriteInt32(m.HairColor);
            w.WriteInt32(m.Face);
        });

        public static CreateCharacterMsg DecodeCreateCharacter(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new CreateCharacterMsg
            {
                Name = r.ReadString(),
                ClassId = r.ReadInt32(),
                Race = r.ReadInt32(),
                Sex = r.ReadInt32(),
                HairStyle = r.ReadInt32(),
                HairColor = r.ReadInt32(),
                Face = r.ReadInt32()
            };
        }

        public static byte[] EncodeCreateResult(CreateResultMsg m) => Write(w =>
        {
            w.WriteByte(m.Ok ? (byte)1 : (byte)0);
            w.WriteString(m.Code);
            w.WriteString(m.Message);
            w.WriteByte(m.Character != null ? (byte)1 : (byte)0);
            if (m.Character != null)
                WriteSummary(w, m.Character);
        });

    }
}
