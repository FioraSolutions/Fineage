using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

/// <summary>
/// Account and password for "Remember me". The bytes live in the OS vault
/// (macOS Keychain, Windows Credential Manager). This type does not write PlayerPrefs or a file.
/// </summary>
public interface ILoginSecretStore
{
    bool TryLoad(out string account, out string password);

    bool Save(string account, string password);

    bool Clear();
}

public static class LoginSecretCodec
{
    public static byte[] Encode(string account, string password)
    {
        byte[] accountBytes = Encoding.UTF8.GetBytes(account ?? "");
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password ?? "");
        byte[] blob = new byte[8 + accountBytes.Length + passwordBytes.Length];
        WriteLength(blob, 0, accountBytes.Length);
        Buffer.BlockCopy(accountBytes, 0, blob, 4, accountBytes.Length);
        WriteLength(blob, 4 + accountBytes.Length, passwordBytes.Length);
        Buffer.BlockCopy(passwordBytes, 0, blob, 8 + accountBytes.Length, passwordBytes.Length);
        Array.Clear(accountBytes, 0, accountBytes.Length);
        Array.Clear(passwordBytes, 0, passwordBytes.Length);
        return blob;
    }

    public static bool TryDecode(byte[] blob, out string account, out string password)
    {
        account = null;
        password = null;
        if (blob == null || blob.Length < 8)
            return false;

        int accountLength;
        if (!TryReadLength(blob, 0, out accountLength))
            return false;
        if (accountLength < 0 || 4 + accountLength + 4 > blob.Length)
            return false;

        int passwordOffset = 4 + accountLength;
        int passwordLength;
        if (!TryReadLength(blob, passwordOffset, out passwordLength))
            return false;
        if (passwordLength < 0 || passwordOffset + 4 + passwordLength != blob.Length)
            return false;

        account = Encoding.UTF8.GetString(blob, 4, accountLength);
        password = Encoding.UTF8.GetString(blob, passwordOffset + 4, passwordLength);
        return true;
    }

    static void WriteLength(byte[] blob, int offset, int length)
    {
        blob[offset] = (byte)length;
        blob[offset + 1] = (byte)(length >> 8);
        blob[offset + 2] = (byte)(length >> 16);
        blob[offset + 3] = (byte)(length >> 24);
    }

    static bool TryReadLength(byte[] blob, int offset, out int length)
    {
        length = 0;
        if (offset < 0 || offset + 4 > blob.Length)
            return false;
        length = blob[offset]
            | (blob[offset + 1] << 8)
            | (blob[offset + 2] << 16)
            | (blob[offset + 3] << 24);
        return true;
    }
}

public static class LoginSecretStore
{
    public const string OsServiceName = "Fineage Login";

    public static ILoginSecretStore Create()
    {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        return new MacKeychainLoginSecretStore();
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        return new WindowsCredentialLoginSecretStore();
#else
        return new UnavailableLoginSecretStore();
#endif
    }
}

/// <summary>In-memory stand-in for editmode tests. Not used by the login window.</summary>
public sealed class MemoryLoginSecretStore : ILoginSecretStore
{
    byte[] _blob;

    public bool TryLoad(out string account, out string password)
    {
        if (_blob == null)
        {
            account = null;
            password = null;
            return false;
        }

        return LoginSecretCodec.TryDecode(_blob, out account, out password);
    }

    public bool Save(string account, string password)
    {
        if (_blob != null)
            Array.Clear(_blob, 0, _blob.Length);
        _blob = LoginSecretCodec.Encode(account, password);
        return true;
    }

    public bool Clear()
    {
        if (_blob != null)
            Array.Clear(_blob, 0, _blob.Length);
        _blob = null;
        return true;
    }
}

public sealed class UnavailableLoginSecretStore : ILoginSecretStore
{
    public bool TryLoad(out string account, out string password)
    {
        account = null;
        password = null;
        return false;
    }

    public bool Save(string account, string password)
    {
        Debug.LogError(
            "[LoginSecret] This platform has no OS secret store (macOS Keychain or Windows Credential Manager). " +
            "The account and password were not saved.");
        return false;
    }

    public bool Clear()
    {
        return true;
    }
}

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
sealed class MacKeychainLoginSecretStore : ILoginSecretStore
{
    const int SecSuccess = 0;
    const int SecItemNotFound = -25300;

    static readonly byte[] Service = Encoding.UTF8.GetBytes(LoginSecretStore.OsServiceName);
    static readonly byte[] AccountKey = Encoding.UTF8.GetBytes("login");

    public bool TryLoad(out string account, out string password)
    {
        account = null;
        password = null;
        uint length;
        IntPtr data;
        IntPtr item;
        int status = SecKeychainFindGenericPassword(
            IntPtr.Zero,
            (uint)Service.Length,
            Service,
            (uint)AccountKey.Length,
            AccountKey,
            out length,
            out data,
            out item);
        if (status == SecItemNotFound)
        {
            ReleaseItem(item);
            return false;
        }
        if (status != SecSuccess || data == IntPtr.Zero)
        {
            Debug.LogError("[LoginSecret] Keychain read failed, status " + status + ".");
            ReleaseItem(item);
            return false;
        }

        byte[] blob = new byte[length];
        Marshal.Copy(data, blob, 0, (int)length);
        SecKeychainItemFreeContent(IntPtr.Zero, data);
        ReleaseItem(item);
        bool decoded = LoginSecretCodec.TryDecode(blob, out account, out password);
        Array.Clear(blob, 0, blob.Length);
        if (!decoded)
            Debug.LogError("[LoginSecret] Keychain item for '" + LoginSecretStore.OsServiceName + "' is not a login secret.");
        return decoded;
    }

    public bool Save(string account, string password)
    {
        DeleteItem();
        byte[] blob = LoginSecretCodec.Encode(account, password);
        IntPtr item;
        int status = SecKeychainAddGenericPassword(
            IntPtr.Zero,
            (uint)Service.Length,
            Service,
            (uint)AccountKey.Length,
            AccountKey,
            (uint)blob.Length,
            blob,
            out item);
        Array.Clear(blob, 0, blob.Length);
        ReleaseItem(item);
        if (status != SecSuccess)
        {
            Debug.LogError("[LoginSecret] Keychain write failed, status " + status + ".");
            return false;
        }

        return true;
    }

    public bool Clear()
    {
        return DeleteItem();
    }

    static bool DeleteItem()
    {
        uint length;
        IntPtr data;
        IntPtr item;
        int status = SecKeychainFindGenericPassword(
            IntPtr.Zero,
            (uint)Service.Length,
            Service,
            (uint)AccountKey.Length,
            AccountKey,
            out length,
            out data,
            out item);
        if (status == SecItemNotFound)
        {
            ReleaseItem(item);
            return true;
        }
        if (status != SecSuccess)
        {
            Debug.LogError("[LoginSecret] Keychain lookup before delete failed, status " + status + ".");
            ReleaseItem(item);
            return false;
        }

        if (data != IntPtr.Zero)
            SecKeychainItemFreeContent(IntPtr.Zero, data);
        int deleted = SecKeychainItemDelete(item);
        if (deleted != SecSuccess)
        {
            Debug.LogError("[LoginSecret] Keychain delete failed, status " + deleted + ".");
            ReleaseItem(item);
            return false;
        }

        return true;
    }

    static void ReleaseItem(IntPtr item)
    {
        if (item != IntPtr.Zero)
            CFRelease(item);
    }

    [DllImport("/System/Library/Frameworks/Security.framework/Security")]
    static extern int SecKeychainFindGenericPassword(
        IntPtr keychain,
        uint serviceNameLength,
        byte[] serviceName,
        uint accountNameLength,
        byte[] accountName,
        out uint passwordLength,
        out IntPtr passwordData,
        out IntPtr itemRef);

    [DllImport("/System/Library/Frameworks/Security.framework/Security")]
    static extern int SecKeychainAddGenericPassword(
        IntPtr keychain,
        uint serviceNameLength,
        byte[] serviceName,
        uint accountNameLength,
        byte[] accountName,
        uint passwordLength,
        byte[] passwordData,
        out IntPtr itemRef);

    [DllImport("/System/Library/Frameworks/Security.framework/Security")]
    static extern int SecKeychainItemDelete(IntPtr itemRef);

    [DllImport("/System/Library/Frameworks/Security.framework/Security")]
    static extern int SecKeychainItemFreeContent(IntPtr attrList, IntPtr data);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void CFRelease(IntPtr cf);
}
#endif

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
sealed class WindowsCredentialLoginSecretStore : ILoginSecretStore
{
    const int CredTypeGeneric = 1;
    const int CredPersistLocalMachine = 2;
    const int ErrorNotFound = 1168;
    const string TargetName = "Fineage/Login";

    public bool TryLoad(out string account, out string password)
    {
        account = null;
        password = null;
        IntPtr native;
        if (!CredRead(TargetName, CredTypeGeneric, 0, out native))
        {
            int error = Marshal.GetLastWin32Error();
            if (error == ErrorNotFound)
                return false;
            Debug.LogError("[LoginSecret] Credential Manager read failed, win32 " + error + ".");
            return false;
        }

        try
        {
            NativeCredential credential = Marshal.PtrToStructure<NativeCredential>(native);
            if (credential.CredentialBlob == IntPtr.Zero || credential.CredentialBlobSize <= 0)
            {
                Debug.LogError("[LoginSecret] Credential Manager item '" + TargetName + "' is empty.");
                return false;
            }

            byte[] blob = new byte[credential.CredentialBlobSize];
            Marshal.Copy(credential.CredentialBlob, blob, 0, blob.Length);
            bool decoded = LoginSecretCodec.TryDecode(blob, out account, out password);
            Array.Clear(blob, 0, blob.Length);
            if (!decoded)
                Debug.LogError("[LoginSecret] Credential Manager item '" + TargetName + "' is not a login secret.");
            return decoded;
        }
        finally
        {
            CredFree(native);
        }
    }

    public bool Save(string account, string password)
    {
        byte[] blob = LoginSecretCodec.Encode(account, password);
        IntPtr blobNative = Marshal.AllocHGlobal(blob.Length);
        IntPtr target = IntPtr.Zero;
        try
        {
            Marshal.Copy(blob, 0, blobNative, blob.Length);
            target = Marshal.StringToHGlobalUni(TargetName);
            NativeCredential credential = new NativeCredential();
            credential.Type = CredTypeGeneric;
            credential.TargetName = target;
            credential.CredentialBlobSize = blob.Length;
            credential.CredentialBlob = blobNative;
            credential.Persist = CredPersistLocalMachine;
            if (!CredWrite(ref credential, 0))
            {
                Debug.LogError("[LoginSecret] Credential Manager write failed, win32 " + Marshal.GetLastWin32Error() + ".");
                return false;
            }

            return true;
        }
        finally
        {
            Array.Clear(blob, 0, blob.Length);
            Marshal.FreeHGlobal(blobNative);
            if (target != IntPtr.Zero)
                Marshal.FreeHGlobal(target);
        }
    }

    public bool Clear()
    {
        if (CredDelete(TargetName, CredTypeGeneric, 0))
            return true;
        int error = Marshal.GetLastWin32Error();
        if (error == ErrorNotFound)
            return true;
        Debug.LogError("[LoginSecret] Credential Manager delete failed, win32 " + error + ".");
        return false;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct NativeCredential
    {
        public int Flags;
        public int Type;
        public IntPtr TargetName;
        public IntPtr Comment;
        public long LastWritten;
        public int CredentialBlobSize;
        public IntPtr CredentialBlob;
        public int Persist;
        public int AttributeCount;
        public IntPtr Attributes;
        public IntPtr TargetAlias;
        public IntPtr UserName;
    }

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern bool CredWrite(ref NativeCredential credential, int flags);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern bool CredRead(string target, int type, int reservedFlag, out IntPtr credential);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern bool CredDelete(string target, int type, int flags);

    [DllImport("advapi32.dll")]
    static extern void CredFree(IntPtr credential);
}
#endif
