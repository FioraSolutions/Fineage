using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace Fineage.Net.Contract
{
    public static partial class NetCodec
    {
        public static CreateResultMsg DecodeCreateResult(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            var msg = new CreateResultMsg
            {
                Ok = r.ReadByte() != 0,
                Code = r.ReadString(),
                Message = r.ReadString()
            };
            if (r.ReadByte() != 0)
                msg.Character = ReadSummary(ref r);
            return msg;
        }

        public static byte[] EncodeSelectCharacter(SelectCharacterMsg m) => Write(w =>
        {
            w.WriteInt32(m.CharacterId);
            w.WriteInt32(m.Slot);
            w.WriteString(m.Name);
        });

        public static SelectCharacterMsg DecodeSelectCharacter(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new SelectCharacterMsg
            {
                CharacterId = r.ReadInt32(),
                Slot = r.ReadInt32(),
                Name = r.ReadString()
            };
        }

        public static byte[] EncodeDeleteCharacter(DeleteCharacterMsg m) => Write(w =>
        {
            w.WriteInt32(m.Slot);
            w.WriteInt32(m.CharacterId);
        });

        public static DeleteCharacterMsg DecodeDeleteCharacter(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new DeleteCharacterMsg { Slot = r.ReadInt32(), CharacterId = r.ReadInt32() };
        }

        public static byte[] EncodeRestoreCharacter(RestoreCharacterMsg m) => Write(w =>
        {
            w.WriteInt32(m.Slot);
            w.WriteInt32(m.CharacterId);
        });

        public static RestoreCharacterMsg DecodeRestoreCharacter(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new RestoreCharacterMsg { Slot = r.ReadInt32(), CharacterId = r.ReadInt32() };
        }

        public static byte[] EncodeDeleteResult(DeleteResultMsg m) => Write(w =>
        {
            w.WriteByte(m.Ok ? (byte)1 : (byte)0);
            w.WriteString(m.Code);
            w.WriteString(m.Message);
            w.WriteInt32(m.Slot);
            w.WriteInt32(m.CharacterId);
        });

        public static DeleteResultMsg DecodeDeleteResult(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new DeleteResultMsg
            {
                Ok = r.ReadByte() != 0,
                Code = r.ReadString(),
                Message = r.ReadString(),
                Slot = r.ReadInt32(),
                CharacterId = r.ReadInt32()
            };
        }

        public static byte[] EncodeHandoff(HandoffMsg m) => Write(w =>
        {
            w.WriteString(m.Account);
            w.WriteInt32(m.CharacterId);
            w.WriteString(m.CharacterName);
            w.WriteInt32(m.ClassId);
            w.WriteInt32(m.Race);
            w.WriteInt32(m.Sex);
            w.WriteString(m.Token);
            w.WriteInt64(m.ExpiresUnixSeconds);
            w.WriteString(m.ZoneHost);
            w.WriteInt32(m.ZonePort);
        });

        public static HandoffMsg DecodeHandoff(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new HandoffMsg
            {
                Account = r.ReadString(),
                CharacterId = r.ReadInt32(),
                CharacterName = r.ReadString(),
                ClassId = r.ReadInt32(),
                Race = r.ReadInt32(),
                Sex = r.ReadInt32(),
                Token = r.ReadString(),
                ExpiresUnixSeconds = r.ReadInt64(),
                ZoneHost = r.ReadString(),
                ZonePort = r.ReadInt32()
            };
        }

        public static byte[] EncodeZoneAuth(ZoneAuthMsg m) => Write(w =>
        {
            w.WriteString(m.Account);
            w.WriteString(m.Token);
        });

        public static ZoneAuthMsg DecodeZoneAuth(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new ZoneAuthMsg { Account = r.ReadString(), Token = r.ReadString() };
        }

        public static byte[] EncodeZoneAuthResult(ZoneAuthResultMsg m) => Write(w =>
        {
            w.WriteByte(m.Ok ? (byte)1 : (byte)0);
            w.WriteString(m.Code);
            w.WriteString(m.Message);
            w.WriteUInt64(m.EntityValue);
            w.WriteUInt32(m.EntityGeneration);
        });

        public static ZoneAuthResultMsg DecodeZoneAuthResult(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new ZoneAuthResultMsg
            {
                Ok = r.ReadByte() != 0,
                Code = r.ReadString(),
                Message = r.ReadString(),
                EntityValue = r.ReadUInt64(),
                EntityGeneration = r.ReadUInt32()
            };
        }

        public static byte[] EncodeEnterWorld(EnterWorldMsg _) => Array.Empty<byte>();
        public static EnterWorldMsg DecodeEnterWorld(ReadOnlySpan<byte> _) => new();
    }
}
