using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Maverick.Agent
{
    public static class CryptoFunction
    {
        private static readonly byte[] keyAES = Encoding.UTF8.GetBytes("GioToiLaiLangThang_TinhYeuThiMienMan");
        private static readonly byte[] keyXOR = Encoding.UTF8.GetBytes("AT19N");
        private static readonly byte[] SecretKey = Encoding.UTF8.GetBytes("MatNaiChaChaCha");

        public static string GetRequestHeaderVallue()
        {
            return GetHash(SecretKey);
        }

        private static byte[] GenerateIV()
        {
            byte[] iv = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv);
            }
            return iv;
        }

        public static string GetHash(byte[] sc)
        {
            return BitConverter.ToString(SHA256.Create().ComputeHash(sc)).Replace("-", "").ToLowerInvariant();
        }

        public static async Task<byte[]> EncryptAESPayload(byte[] payload)
        {
            try
            {
                var IV = GenerateIV();
                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyAES;
                    aes.IV = IV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        using (var ms = new MemoryStream())
                        {
                            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                            {
                                ms.Write(IV, 0, IV.Length); // Prepend IV to the ciphertext
                                await cs.WriteAsync(payload, 0, payload.Length);
                                cs.FlushFinalBlock();
                                return ms.ToArray();
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw new InvalidOperationException("Encryption failed.", ex);
            }

        }

        public static async Task<byte[]> DecryptAESPayload(byte[] payload)
        {
            try
            {
                byte[] iv = payload.Take(16).ToArray();
                byte[] cipherText = payload.Skip(16).ToArray();
                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyAES;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        using (var ms = new MemoryStream(cipherText))
                        {
                            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                            {
                                using (var resultStream = new MemoryStream())
                                {
                                    await cs.CopyToAsync(resultStream);
                                    return resultStream.ToArray();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Decryption failed.", ex);
            }
        }

        public static byte[] XorPayload(byte[] payload)
        {
            byte[] result = new byte[payload.Length];
            for (int i = 0; i < payload.Length; i++)
            {
                result[i] = (byte)(payload[i] ^ keyXOR[i % keyXOR.Length]);
            }
            return result;
        }
    }
}
