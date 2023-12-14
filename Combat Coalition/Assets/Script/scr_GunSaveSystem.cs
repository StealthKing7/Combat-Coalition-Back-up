using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class scr_GunSaveSystem : MonoBehaviour
{

    public static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";



    public static void Save(string json, Texture2D screenshot)
    {
        byte[] jsonByteArray = Encoding.Unicode.GetBytes(json);
        byte[] screenshotByteArray = screenshot.EncodeToPNG();


        SaveFile.Header header = new SaveFile.Header
        {
            jsonByteSize = jsonByteArray.Length
        };
        string headerJson = JsonUtility.ToJson(header);
        byte[] headerJsonByteArray = Encoding.Unicode.GetBytes(headerJson);

        ushort headerSize = (ushort)headerJsonByteArray.Length;
        byte[] headerSizeByteArray = BitConverter.GetBytes(headerSize);

        List<byte> byteList = new List<byte>();
        byteList.AddRange(headerSizeByteArray);
        byteList.AddRange(headerJsonByteArray);
        byteList.AddRange(jsonByteArray);
        byteList.AddRange(screenshotByteArray);

        string filename = "Save_" + ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() + ".weaponsave";
        string filePath = SAVE_FOLDER + filename;

        File.WriteAllBytes(filePath, byteList.ToArray());
    }

    public static List<string> GetSaveFilenameList()
    {
        List<string> saveFilenameList = new List<string>();

        foreach (string fullPathFilename in Directory.GetFiles(SAVE_FOLDER))
        {
            if (fullPathFilename.Contains(".meta")) continue; // Ignore meta files
            string filename = fullPathFilename.Remove(0, SAVE_FOLDER.Length);
            saveFilenameList.Add(filename);
        }

        return saveFilenameList;
    }

    public static void Load(string filename, out string json, out Texture2D screenshotTexture2D)
    {
        string filePath = SAVE_FOLDER + filename;
        byte[] byteArray = File.ReadAllBytes(filePath);
        List<byte> byteList = new List<byte>(byteArray);

        ushort headerSize = BitConverter.ToUInt16(new byte[] { byteArray[0], byteArray[1] }, 0);
        List<byte> headerByteList = byteList.GetRange(2, headerSize);
        string headerJson = Encoding.Unicode.GetString(headerByteList.ToArray());
        SaveFile.Header header = JsonUtility.FromJson<SaveFile.Header>(headerJson);

        List<byte> jsonByteList = byteList.GetRange(2 + headerSize, header.jsonByteSize);
        string gameDataJson = Encoding.Unicode.GetString(jsonByteList.ToArray());
        json = gameDataJson;

        int startIndex = 2 + headerSize + header.jsonByteSize;
        int endIndex = byteArray.Length - startIndex;
        List<byte> screenshotByteList = byteList.GetRange(startIndex, endIndex);
        screenshotTexture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        screenshotTexture2D.LoadImage(screenshotByteList.ToArray());
    }




    [Serializable]
    private class SaveFile
    {

        [Serializable]
        public class Header
        {

            public int jsonByteSize;

        }

    }
}
