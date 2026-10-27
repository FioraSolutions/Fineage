using System.Buffers.Binary;
using Fineage.LoginStub;
using Fineage.LoginStub.Crypto;
using Xunit;

namespace Fineage.LoginStub.Tests;

public class LoginCryptTests
{
    [Fact]
    public void BlowfishProcess_EncryptDecrypt_RoundTrips()
    {
        byte[] key = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                      0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10];
        byte[] plain = [0xDE, 0xAD, 0xBE, 0xEF, 0xCA, 0xFE, 0xBA, 0xBE];

        byte[] encrypted = LoginCrypt.BlowfishProcess(key, encrypt: true, plain);
        byte[] decrypted = LoginCrypt.BlowfishProcess(key, encrypt: false, encrypted);

        Assert.Equal(plain, decrypted);
    }

    [Fact]
    public void BlowfishProcess_Encrypted_DiffersFromPlain()
    {
        byte[] key = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                      0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10];
        byte[] plain = [0xDE, 0xAD, 0xBE, 0xEF, 0xCA, 0xFE, 0xBA, 0xBE];

        byte[] encrypted = LoginCrypt.BlowfishProcess(key, encrypt: true, plain);
        Assert.NotEqual(plain, encrypted);
    }

    [Fact]
    public void XorPass_EncryptDecrypt_RoundTrips()
    {
        var body = new PacketBuilder()
            .C(0x00)
            .D(0x11223344)
            .D(0x0000c621)
            .B(new byte[128])
            .D(1).D(2).D(3).D(4)
            .B(Enumerable.Repeat((byte)0xAB, 16).ToArray())
            .C(0x00)
            .D(0)
            .H(0)
            .ToArray();

        byte[] original = (byte[])body.Clone();

        int xorKey = unchecked((int)0xA1B2C3D4);
        byte[] encrypted = LoginCrypt.EncryptInit(body, xorKey);
        byte[] decrypted = LoginCrypt.DecryptLogin(encrypted, LoginCrypt.StaticBlowfishKey, init: true);

        Assert.Equal(0x00, decrypted[0]);
        int session = BinaryPrimitives.ReadInt32LittleEndian(decrypted.AsSpan(1));
        Assert.Equal(0x11223344, session);
    }

    [Fact]
    public void AppendChecksum_VerifyChecksum_Succeeds()
    {
        byte[] data = new byte[16];
        data[0] = 0x03;
        data[1] = 10;

        LoginCrypt.AppendChecksum(data);
        Assert.True(LoginCrypt.VerifyChecksum(data));
    }

    [Fact]
    public void VerifyChecksum_CorruptedData_Fails()
    {
        byte[] data = new byte[16];
        data[0] = 0x03;
        LoginCrypt.AppendChecksum(data);

        data[0] = 0xFF;
        Assert.False(LoginCrypt.VerifyChecksum(data));
    }

    [Fact]
    public void VerifyChecksum_TooShort_Fails()
    {
        Assert.False(LoginCrypt.VerifyChecksum(new byte[3]));
        Assert.False(LoginCrypt.VerifyChecksum(new byte[4]));
    }

    [Fact]
    public void VerifyChecksum_NotAligned_Fails()
    {
        Assert.False(LoginCrypt.VerifyChecksum(new byte[5]));
    }

    [Fact]
    public void Pad8_AlreadyAligned_ReturnsSame()
    {
        byte[] data = new byte[16];
        var result = LoginCrypt.Pad8(data);
        Assert.Same(data, result);
    }

    [Fact]
    public void Pad8_NotAligned_PadsToMultipleOf8()
    {
        byte[] data = new byte[5];
        var result = LoginCrypt.Pad8(data);
        Assert.Equal(8, result.Length);
        Assert.Equal(data[..5], result[..5]);
    }

    [Fact]
    public void EncryptLogin_DecryptLogin_RoundTrips()
    {
        byte[] key = Enumerable.Range(0, 16).Select(i => (byte)i).ToArray();
        var body = new PacketBuilder().C(0x03).D(10).D(20).ToArray();

        byte[] encrypted = LoginCrypt.EncryptLogin(body, key);
        byte[] decrypted = LoginCrypt.DecryptLogin(encrypted, key, init: false);

        Assert.Equal(0x03, decrypted[0]);
    }

    [Fact]
    public void ScrambleModulus_ProducesDifferentOutput()
    {
        byte[] modulus = new byte[128];
        for (int i = 0; i < modulus.Length; i++)
            modulus[i] = (byte)(i + 1);

        byte[] scrambled = LoginCrypt.ScrambleModulus(modulus);

        Assert.Equal(128, scrambled.Length);
        Assert.NotEqual(modulus, scrambled);
    }

    [Fact]
    public void StaticBlowfishKey_IsCorrectLength()
    {
        Assert.Equal(16, LoginCrypt.StaticBlowfishKey.Length);
    }

    [Fact]
    public void Concat_JoinsArrays()
    {
        byte[] a = [1, 2, 3];
        byte[] b = [4, 5];
        byte[] result = LoginCrypt.Concat(a, b);

        Assert.Equal([1, 2, 3, 4, 5], result);
    }
}
