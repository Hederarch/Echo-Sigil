using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Windows.Forms;
using System;

namespace MapEditor
{
    public static class SaveSystem
    {

        public static bool developerMode = true;
        public const string mapFileName = "Quests";
        public const string implementFileName = "Implements";
        public const string pallateFileName = "Pallate";
        public const string baseSpriteFileName = "Base";
        public const string defaultQuest = "Test";

        static ModPath[] modPaths;

        public static ModPath[] GetModPaths(bool reloadModPaths = false)
        {
            if (modPaths == null || reloadModPaths)
            {
                ModPath[] newModPaths = new ModPath[1];
                newModPaths[0] = GetDefualtModPath();
                modPaths = newModPaths;
            }
            return modPaths;
        }

        private static ModPath GetDefualtModPath()
        {
            return UnityEngine.Application.dataPath + "/Offical Story Pack";

        }

        public static bool SaveMap(Map map)
        {
            if (map.readyForSave)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(GetMapPath(map.modPathIndex, map.quest, map.name), FileMode.Create);
                formatter.Serialize(stream, map);
                stream.Close();
                return true;
            } 
            else
            {
                Debug.LogWarning("Map was not ready for saveing");
                return false;
            }

        }

        public static string GetMapPath(int modPathIndex, string quest, string name) => GetQuestPath(modPathIndex, quest) + "/" + name;

        /*public static Map LoadMap(int modPathindex, string quest, string name, bool logError = false)
        {
            string path = GetMapPath(modPathindex, quest, name);
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                Map map = formatter.Deserialize(stream) as Map;
                stream.Close();
                map.modPathIndex = modPathindex;
                return map;
            }
            else if (logError)
            {
                Debug.LogError(name + " not found at " + path);
            }
            return new Map();
        }*/

        public static void DeleteMap(int modPathindex, string quest, string name, bool logError = false)
        {
            string path = GetMapPath(modPathindex, quest, name);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (logError)
            {
                Debug.LogWarning(name + " not found in " + path + ". So... I guess its been sucsessfuly gotten rid of then.");
            }
        }

        public static void SavePallate(int modPathIndex, string quest, Sprite[] pallate)
        {
            string path = GetPallatePath(modPathIndex, quest);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            for (int i = 0; i < pallate.Length; i++)
            {
                string imagePath = path + i + ".png";
                if (!File.Exists(imagePath))
                {
                    SavePNG(imagePath, pallate[i].texture);
                }
            }
        }

        public static string GetQuestPath(int modPathIndex, string quest) => GetModPaths()[modPathIndex] + "/" + mapFileName + "/" + quest;

        public static string GetPallatePath(int modPathIndex, string quest) => GetQuestPath(modPathIndex, quest) + "/" + pallateFileName;

        public static Texture2D[] LoadPallate(int modPathIndex, string quest = defaultQuest)
        {
            string path = GetPallatePath(modPathIndex, quest);
            List<Texture2D> spritePallate = new List<Texture2D>();
            if (!Directory.Exists(path))
            {
                Debug.LogError("Pallate folder does not exist in " + quest + " directory. Are you sure this is a quest folder?");
                return null;
            }
            for (int i = 0; File.Exists(path + i + ".png"); i++)
            {
                spritePallate.Add(LoadPNG(path + i + ".png"));
            }
            return spritePallate.ToArray();
        }

        public static Texture2D LoadPNG(string filePath)
        {
            if (File.Exists(filePath))
            {
                byte[] fileData;
                fileData = File.ReadAllBytes(filePath);
                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, true);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.

                if (tex.width != tex.height)
                {
                    Debug.LogWarning("Texture " + filePath + " is not square. Texture hieght may be adversly affected");
                }

                return tex;
            }
            return null;
        }

        public static void SavePNG(string filePath, Texture2D texture)
        {
            string fullName = Directory.GetParent(filePath).FullName;
            if (!Directory.Exists(fullName))
            {
                Directory.CreateDirectory(fullName);
            }
            if (!texture.isReadable)
            {
                Debug.LogError(texture + " is unreadable");
                return;
            }
            byte[] fileData;
            fileData = texture.EncodeToPNG();
            File.WriteAllBytes(filePath, fileData);
        }

    }
}

public struct ModPath
{
    public string modPath;
    public string modName;

    public ModPath(string _modPath)
    {
        modPath = _modPath;
        modName = System.IO.Path.GetFileName(_modPath);
    }

    public static implicit operator ModPath(string s) => new ModPath(s);
    public static implicit operator string(ModPath m) => m.modPath;

}
