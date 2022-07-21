using System.Security.Cryptography;

namespace InsightArchitectures.Utilities;

#pragma warning disable SA1600
internal static class Crypto
{
    public static byte[] Hash_SHA256(byte[] data)
    {
        using var sha256 = SHA256.Create();

        return sha256.ComputeHash(data);
    }

    public static byte[] SignHMAC_SHA256(byte[] data, byte[] key)
    {
        if (key == null || key.Length == 0)
        {
            throw new ArgumentNullException(nameof(key), "Please specify a Secret Signing Key.");
        }

        if (data == null || data.Length == 0)
        {
            throw new ArgumentNullException(nameof(data), "Please specify data to sign.");
        }

        using var hmac = new HMACSHA256();
        hmac.Key = key;

        return hmac.ComputeHash(data);
    }
}
