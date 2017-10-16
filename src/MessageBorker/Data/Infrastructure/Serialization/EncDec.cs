using System;
using System.IO;
using System.Security.Cryptography;

public class EncDec
{
    public static void Encrypt(Stream clearData, string Password)
    {
        PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
            new byte[]
            {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d,
                0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
            });
        var encrypt = Encrypt(ToByteArray(clearData), pdb.GetBytes(32), pdb.GetBytes(16));
        clearData.Position = 0;
        clearData.Write(encrypt, 0, encrypt.Length); 
    }

    public static void Decrypt(Stream cipherData, string Password)
    {
        PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
            new byte[]
            {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d,
                0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
            });
        var decrypt = Decrypt(ToByteArray(cipherData), pdb.GetBytes(32), pdb.GetBytes(16));
        cipherData.Position = 0;
        cipherData.Write(decrypt, 0, decrypt.Length); 
        cipherData.Position = 0;
    }
    
    public static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
    {
        var ms = new MemoryStream();
        var alg = Rijndael.Create();
        alg.Key = Key;
        alg.IV = IV;
        var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(clearData, 0, clearData.Length);
        cs.Close();
        return ms.ToArray();
    }
    
    public static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
    {
        var ms = new MemoryStream();
        var alg = Rijndael.Create();
        alg.Key = Key;
        alg.IV = IV;
        var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(cipherData, 0, cipherData.Length);
        cs.Close();
        return  ms.ToArray();
    }
    
    private static byte[] ToByteArray(Stream stream)
    {
        stream.Position = 0;
        byte[] buffer = new byte[stream.Length];
        for (int totalBytesCopied = 0; totalBytesCopied < stream.Length;)
            totalBytesCopied +=
                stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
        return buffer;
    }
}