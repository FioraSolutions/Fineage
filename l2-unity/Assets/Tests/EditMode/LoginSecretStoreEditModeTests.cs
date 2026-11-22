using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class LoginSecretStoreEditModeTests
{
    [Test]
    public void Codec_roundtrip_keeps_account_and_password()
    {
        byte[] blob = LoginSecretCodec.Encode("Fineage", "p@ss\u0000word");
        string account;
        string password;
        Assert.IsTrue(LoginSecretCodec.TryDecode(blob, out account, out password));
        Assert.AreEqual("Fineage", account);
        Assert.AreEqual("p@ss\u0000word", password);
    }

    [Test]
    public void Codec_rejects_a_truncated_blob()
    {
        byte[] blob = LoginSecretCodec.Encode("acc", "secret");
        byte[] truncated = new byte[blob.Length - 1];
        System.Array.Copy(blob, truncated, truncated.Length);
        string account;
        string password;
        Assert.IsFalse(LoginSecretCodec.TryDecode(truncated, out account, out password));
    }

    [Test]
    public void Memory_store_drops_account_and_password_together()
    {
        MemoryLoginSecretStore store = new MemoryLoginSecretStore();
        Assert.IsTrue(store.Save("account", "password"));
        string account;
        string password;
        Assert.IsTrue(store.TryLoad(out account, out password));
        Assert.AreEqual("account", account);
        Assert.AreEqual("password", password);

        Assert.IsTrue(store.Clear());
        Assert.IsFalse(store.TryLoad(out account, out password));
        Assert.IsNull(account);
        Assert.IsNull(password);
    }

    [Test]
    public void Platforms_without_an_os_vault_do_not_save()
    {
        LogAssert.Expect(
            LogType.Error,
            new System.Text.RegularExpressions.Regex("no OS secret store"));
        UnavailableLoginSecretStore store = new UnavailableLoginSecretStore();
        Assert.IsFalse(store.Save("account", "password"));
        string account;
        string password;
        Assert.IsFalse(store.TryLoad(out account, out password));
    }
}
