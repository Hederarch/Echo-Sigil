using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

public static class SaveSystem
{
   public static void SaveMap(string name, Map map)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Maps/"+ name + ".hedrap";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, map);
        stream.Close();
    }
    public static Map LoadMap(string name, bool logError = false)
    {
        string path = Application.persistentDataPath + "/Maps/" + name + ".hedrap";
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
    public static void DeleteMap(string name, bool logError = false)
    {
        string path = Application.persistentDataPath + "/Maps/" + name + ".hedrap";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Truncate);
            stream.Close();
        }
        else if(logError)
        {
            Debug.LogError("Map File not found in " + path + ". So... um, I guess its been sucsessfuly gotten rid of then.");
        }
    }
}
