using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Security;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{

    private string key = "testKey";
    private string path = Directory.GetCurrentDirectory() + "\\Save\\";
    private string fileName = "test.sav";

    public void Start()
    {

    }
    

    public void Save()
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        StreamWriter writer = new StreamWriter(path + fileName, false);
        

        CarAutoControl carAI = GameObject.Find("Car").GetComponent<CarAutoControl>();
        if(carAI == null)
        {
            Debug.LogError("Error! CarAutoControl object not found!");
        }
        writer.WriteLine(Encrypt("startingNode:" + carAI.startingNode.ToString()));
        writer.WriteLine(Encrypt("targetNode:" + carAI.targetNode.ToString()));
        Vector3 rotation = carAI.transform.rotation.eulerAngles;
        Debug.Log("rotation y:" + rotation.y.ToString());
        writer.WriteLine(Encrypt("rotation:" + rotation.y.ToString()));

        writer.Close();
    }

    public void Load()
    {
        //Load scene first.
        Debug.Log("Load");
        StreamReader reader = new StreamReader(path + fileName);
        string read, decrypt;
        GameObject car = GameObject.Find("Car");

        if (car == null)
        {
            Debug.LogError("Error! Car object not found!");
        }
        CarAutoControl carAI = GameObject.Find("Car").GetComponent<CarAutoControl>();
        if (carAI == null)
        {
            Debug.LogError("Error! CarAutoControl object not found!");
        }
        float rotation=0;
        while (!reader.EndOfStream)
        {
            read = reader.ReadLine();
            decrypt = Decrypt(read);
            string[] elements = decrypt.Split(':');
            if(elements.Length<2)
            {
                Debug.LogError("Error! Load data invalid!");
            }
            switch(elements[0])
            {
                case "startingNode":
                    carAI.startingNode = Int32.Parse(elements[1]);
                    break;

                case "targetNode":
                    carAI.targetNode = Int32.Parse(elements[1]);
                    break;

                case "rotation":
                    rotation = float.Parse(elements[1]);
                    Debug.Log("rotation y:" + rotation.ToString());
                    break;

                default:
                    break;
            }
        }

        carAI.InitState();
        carAI.transform.rotation = Quaternion.Euler(0, rotation, 0);

        reader.Close();
    }

    private string Encrypt(string stringToEncrypt)
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

    private string Decrypt(string stringToDecrypt)
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
