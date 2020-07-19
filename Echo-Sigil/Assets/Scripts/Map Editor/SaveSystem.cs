using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.WSA;
using System;

public static class SaveSystem
{
    public static void SaveMap(string path, Map map)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, map);
        stream.Close();
    }

    public static Map LoadMap(string path, bool logError = false)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Map map = formatter.Deserialize(stream) as Map;
            stream.Close();
            return map;
        }
        else if (logError)
        {
            Debug.LogError("Map File not found in " + path);
        }
        return null;
    }

    public static Sprite[] LoadPallate(string path)
    {
        List<Sprite> spritePallate = new List<Sprite>();
        if (!Directory.Exists(path + "/Pallate"))
        {
            Debug.LogError("Pallate folder does not exist in " + path + " directory. Are you sure this is a quest folder?");
            return null;
        }
        for (int i = 0; File.Exists(path + "/Pallate/" + i + ".png"); i++)
        {
            spritePallate.Add(LoadPNG(path + "/Pallate/" + i + ".png", Vector2.one / 2f));
        }
        return spritePallate.ToArray();
    }

    internal static void SavePallate(string fullName, Sprite[] pallate)
    {
        if (!Directory.Exists(fullName + "/Pallate"))
        {
            Directory.CreateDirectory(fullName + "/Pallate");
        }
        for(int i = 0; i < pallate.Length; i++)
        {
            if(!File.Exists(fullName + "/Pallate/" + i + ".png"))
            {
                SavePNG(fullName + "/Pallate/" + i + ".png", pallate[i].texture);
            }
        }
    }

    public static Sprite LoadPNG(string filePath, Vector2 pivot)
    {
        if (File.Exists(filePath))
        {
            byte[] fileData;
            fileData = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.

            if (tex.width != tex.height)
            {
                Debug.LogWarning("Texture " + filePath + "is not square. Texture hieght may be adversly affected");
            }

            Sprite sprite = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)), pivot, tex.width);
            return sprite;
        }
        return null;
    }

    public static void SavePNG(string filePath, Texture2D texture)
    {
        byte[] fileData;
        fileData = texture.EncodeToJPG();
        File.WriteAllBytes(filePath, fileData);
    }

    public static void DeleteMap(string path, bool logError = false)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else if (logError)
        {
            Debug.LogError("Map File not found in " + path + ". So... um, I guess its been sucsessfuly gotten rid of then.");
        }
    }

}