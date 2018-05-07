using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Security;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{

    private string key = "test";
    private string path = Directory.GetCurrentDirectory() + "\\Save\\";
    private string fileName = "test.sav";


    public void Save(List<string> data)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        StreamWriter writer = new StreamWriter(path + fileName, true);
        string encrypt;
        for (int i = 0; i < data.Count; i++)
        {
            encrypt = Encrypt(data[i], key);
            writer.WriteLine(encrypt);
        }
        writer.Close();
    }

    public List<string> Load()
    {
        StreamReader reader = new StreamReader(path + fileName);
        string read;
        List<string> data = null;
        while (!reader.EndOfStream)
        {
            read = reader.ReadLine();
            data.Add(Decrypt(read, key));
        }
        return data;
    }

    private string Encrypt(string stringToEncrypt, string key)
    {
        if (string.IsNullOrEmpty(stringToEncrypt))
        {
            Debug.LogError("Error! stringToEncrypt is null!");
        }
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("Error! key is null!");
        }

        System.Security.Cryptography.CspParameters cspp = new System.Security.Cryptography.CspParameters();
        cspp.KeyContainerName = key;
        System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider(cspp);
        rsa.PersistKeyInCsp = true;

        byte[] bytes = rsa.Encrypt(System.Text.UTF8Encoding.UTF8.GetBytes(stringToEncrypt), true);
        return BitConverter.ToString(bytes);
    }

    private string Decrypt(string stringToDecrypt, string key)
    {
        if (string.IsNullOrEmpty(stringToDecrypt))
        {
            Debug.LogError("Error! stringToDecrypt is null!");
        }
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("Error! key is null!");
        }

        string result = null;
        System.Security.Cryptography.CspParameters cspp = new System.Security.Cryptography.CspParameters();
        cspp.KeyContainerName = key;

        System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider(cspp);
        rsa.PersistKeyInCsp = true;

        string[] decryptArray = stringToDecrypt.Split(new string[] { "-" }, StringSplitOptions.None);
        byte[] decryptByteArray = Array.ConvertAll<string, byte>(decryptArray, (s => Convert.ToByte(byte.Parse(s, System.Globalization.NumberStyles.HexNumber))));

        byte[] bytes = rsa.Decrypt(decryptByteArray, true);

        result = System.Text.UTF8Encoding.UTF8.GetString(bytes);

        return result;
    }
}
