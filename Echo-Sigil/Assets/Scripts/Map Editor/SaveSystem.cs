using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using UnityEditor;
using System.Collections.Generic;

public static class SaveSystem
{
    public static void SaveMap(string path, Map map)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, map);
        stream.Close();
    }
    public static Map LoadMap(string path,bool logError = false)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Map map = formatter.Deserialize(stream) as Map;
            stream.Close();
            return map;
        }
        else if(logError)
        {
            Debug.LogError("Map File not found in " + path);
        }
        return null;
    }
    public static Sprite[] LoadPallate(string path,bool logError = false)
    {
        List<Sprite> spritePallate = new List<Sprite>();
        return spritePallate.ToArray();
    }
    public static void DeleteMap(string path, bool logError = false)
    {
        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Truncate);
            stream.Close();
        }
        else if(logError)
        {
            Debug.LogError("Map File not found in " + path + ". So... um, I guess its been sucsessfuly gotten rid of then.");
        }
    }
}
